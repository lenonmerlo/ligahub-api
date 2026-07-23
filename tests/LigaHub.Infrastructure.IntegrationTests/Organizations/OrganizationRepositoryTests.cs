using LigaHub.Domain.Organizations;
using LigaHub.Infrastructure.IntegrationTests.Database;
using LigaHub.Infrastructure.Persistence;
using LigaHub.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

namespace LigaHub.Infrastructure.IntegrationTests.Organizations;

public sealed class OrganizationRepositoryTests
    : IClassFixture<SqlServerFixture>
{
    private readonly SqlServerFixture _fixture;

    public OrganizationRepositoryTests(SqlServerFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task TaskAsync_ShouldPersistOrganization()
    {
        var organization = Organization.Create(
            $"Liga {Guid.NewGuid():N}");

        await using (var dbContext = await CreateDbContextAsync())
        {
            var repository = new OrganizationRepository(dbContext);

            await repository.AddAsync(organization);
        }

        await using var verificationContext =
            await CreateDbContextAsync();

        var persistedOrganization = await verificationContext
            .Organizations
            .AsNoTracking()
            .SingleAsync(item => item.Id == organization.Id);

        Assert.Equal(organization.Id, persistedOrganization.Id);
        Assert.Equal(organization.Name, persistedOrganization.Name);
    }

    [Fact]
    public async Task ExistsByNameAsync_ShouldReturnExpectedResult()
    {
        var organization = Organization.Create(
            $"Liga {Guid.NewGuid():N}");

        await using var dbContext = await CreateDbContextAsync();
        var repository = new OrganizationRepository(dbContext);

        await repository.AddAsync(organization);

        var existingNameFound = await repository.ExistsByNameAsync(
            organization.Name);

        var missingNameFound = await repository.ExistsByNameAsync(
            $"Missing {Guid.NewGuid():N}");

        Assert.True(existingNameFound);
        Assert.False(missingNameFound);
    }

    private async Task<LigaHubDbContext> CreateDbContextAsync()
    {
        var options = new DbContextOptionsBuilder<LigaHubDbContext>()
            .UseSqlServer(_fixture.ConnectionString)
            .Options;

        var dbContext = new LigaHubDbContext(options);

        await dbContext.Database.EnsureCreatedAsync();

        return dbContext;
    }
}
