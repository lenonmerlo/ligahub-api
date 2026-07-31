using LigaHub.Domain.Organizations;
using LigaHub.Domain.Teams;
using LigaHub.Infrastructure.IntegrationTests.Database;
using LigaHub.Infrastructure.Persistence;
using LigaHub.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

namespace LigaHub.Infrastructure.IntegrationTests.Teams;

public sealed class TeamRepositoryTests
    : IClassFixture<SqlServerFixture>
{
    private readonly SqlServerFixture _fixture;

    public TeamRepositoryTests(SqlServerFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task ListAsyncAndCountAsync_ShouldScopeAndPaginateTeams()
    {
        var firstOrganization = Organization.Create(
            $"Liga {Guid.NewGuid():N}");
        var secondOrganization = Organization.Create(
            $"Liga {Guid.NewGuid():N}");

        var firstTeam = Team.Create(
            firstOrganization.Id,
            $"A {Guid.NewGuid():N}");
        var secondTeam = Team.Create(
            firstOrganization.Id,
            $"B {Guid.NewGuid():N}");
        var thirdTeam = Team.Create(
            firstOrganization.Id,
            $"C {Guid.NewGuid():N}");
        var otherOrganizationTeam = Team.Create(
            secondOrganization.Id,
            $"A {Guid.NewGuid():N}");

        await using var dbContext = await CreateDbContextAsync();

        await dbContext.Organizations.AddRangeAsync(
            firstOrganization,
            secondOrganization);
        await dbContext.SaveChangesAsync();

        var repository = new TeamRepository(dbContext);

        await repository.AddAsync(firstTeam);
        await repository.AddAsync(secondTeam);
        await repository.AddAsync(thirdTeam);
        await repository.AddAsync(otherOrganizationTeam);

        var teams = await repository.ListAsync(
            firstOrganization.Id,
            skip: 1,
            take: 1);

        var firstOrganizationCount = await repository.CountAsync(
            firstOrganization.Id);

        var secondOrganizationCount = await repository.CountAsync(
            secondOrganization.Id);

        var listedTeam = Assert.Single(teams);

        Assert.Equal(secondTeam.Id, listedTeam.Id);
        Assert.Equal(3, firstOrganizationCount);
        Assert.Equal(1, secondOrganizationCount);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldScopeTeamToOrganization()
    {
        var firstOrganization = Organization.Create(
            $"Liga {Guid.NewGuid():N}");
        var secondOrganization = Organization.Create(
            $"Liga {Guid.NewGuid():N}");
        var team = Team.Create(
            firstOrganization.Id,
            $"Time {Guid.NewGuid():N}");

        await using var dbContext = await CreateDbContextAsync();

        await dbContext.Organizations.AddRangeAsync(
            firstOrganization,
            secondOrganization);
        await dbContext.SaveChangesAsync();

        var repository = new TeamRepository(dbContext);

        await repository.AddAsync(team);

        var teamInFirstOrganization = await repository.GetByIdAsync(
            firstOrganization.Id,
            team.Id);

        var teamInSecondOrganization = await repository.GetByIdAsync(
            secondOrganization.Id,
            team.Id);

        Assert.NotNull(teamInFirstOrganization);
        Assert.Equal(team.Id, teamInFirstOrganization.Id);
        Assert.Null(teamInSecondOrganization);
    }

    [Fact]
    public async Task AddAsync_ShouldPersistTeam()
    {
        var organization = Organization.Create(
            $"Liga {Guid.NewGuid():N}");
        var team = Team.Create(
            organization.Id,
            $"Time {Guid.NewGuid():N}");

        await using (var dbContext = await CreateDbContextAsync())
        {
            await dbContext.Organizations.AddAsync(organization);
            await dbContext.SaveChangesAsync();

            var repository = new TeamRepository(dbContext);

            await repository.AddAsync(team);
        }

        await using var verificationContext =
            await CreateDbContextAsync();

        var persistedTeam = await verificationContext
            .Teams
            .AsNoTracking()
            .SingleAsync(item => item.Id == team.Id);

        Assert.Equal(team.Id, persistedTeam.Id);
        Assert.Equal(organization.Id, persistedTeam.OrganizationId);
        Assert.Equal(team.Name, persistedTeam.Name);
    }

    [Fact]
    public async Task ExistsByNameAsync_ShouldScopeNameToOrganization()
    {
        var firstOrganization = Organization.Create(
            $"Liga {Guid.NewGuid():N}");
        var secondOrganization = Organization.Create(
            $"Liga {Guid.NewGuid():N}");
        var teamName = $"Time {Guid.NewGuid():N}";
        var team = Team.Create(
            firstOrganization.Id,
            teamName);

        await using var dbContext = await CreateDbContextAsync();

        await dbContext.Organizations.AddRangeAsync(
            firstOrganization,
            secondOrganization);
        await dbContext.SaveChangesAsync();

        var repository = new TeamRepository(dbContext);

        await repository.AddAsync(team);

        var foundInFirstOrganization =
            await repository.ExistsByNameAsync(
                firstOrganization.Id,
                teamName);

        var foundInSecondOrganization =
            await repository.ExistsByNameAsync(
                secondOrganization.Id,
                teamName);

        Assert.True(foundInFirstOrganization);
        Assert.False(foundInSecondOrganization);
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
