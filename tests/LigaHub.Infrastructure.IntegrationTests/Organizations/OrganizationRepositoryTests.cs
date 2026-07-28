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

    [Fact]
    public async Task ListAsync_ShouldReturnOrderedPage()
    {
        await using var dbContext = await CreateDbContextAsync();

        await dbContext.Organizations.ExecuteDeleteAsync();

        var firstOrganization = Organization.Create("Liga A");
        var secondOrganization = Organization.Create("Liga B");
        var thirdOrganization = Organization.Create("Liga C");

        dbContext.Organizations.AddRange(
            thirdOrganization,
            firstOrganization,
            secondOrganization);

        await dbContext.SaveChangesAsync();

        var repository = new OrganizationRepository(dbContext);

        var organizations = await repository.ListAsync(
            skip: 1,
            take: 1);

        var organization = Assert.Single(organizations);

        Assert.Equal(secondOrganization.Id, organization.Id);
        Assert.Equal("Liga B", organization.Name);
    }

    [Fact]
    public async Task CountAsync_ShouldReturnOrganizationCount()
    {
        await using var dbContext = await CreateDbContextAsync();

        await dbContext.Organizations.ExecuteDeleteAsync();

        dbContext.Organizations.AddRange(
            Organization.Create("Liga A"),
            Organization.Create("Liga B"));

        await dbContext.SaveChangesAsync();

        var repository = new OrganizationRepository(dbContext);

        var count = await repository.CountAsync();

        Assert.Equal(2, count);
    }

    [Fact]
    public async Task UpdateAsync_ShouldPersistOrganizationChanges()
    {
        var organization = Organization.Create("Old Name");

        await using (var dbContext = await CreateDbContextAsync())
        {
            await dbContext.Organizations.ExecuteDeleteAsync();

            var repository = new OrganizationRepository(dbContext);

            await repository.AddAsync(organization);

            organization.Rename("New Name");

            await repository.UpdateAsync(organization);
        }

        await using var verificationContext =
            await CreateDbContextAsync();

        var persistedOrganization = await verificationContext
            .Organizations
            .AsNoTracking()
            .SingleAsync(item => item.Id == organization.Id);

        Assert.Equal("New Name", persistedOrganization.Name);
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
