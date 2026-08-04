using LigaHub.Domain.Organizations;
using LigaHub.Domain.Players;
using LigaHub.Domain.Teams;
using LigaHub.Infrastructure.IntegrationTests.Database;
using LigaHub.Infrastructure.Persistence;
using LigaHub.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

namespace LigaHub.Infrastructure.IntegrationTests.Players;

public sealed class PlayerRepositoryTests
    : IClassFixture<SqlServerFixture>
{
    private readonly SqlServerFixture _fixture;

    public PlayerRepositoryTests(SqlServerFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task AddAsync_ShouldPersistPlayer()
    {
        var organization = Organization.Create(
            $"Liga {Guid.NewGuid():N}");
        var team = Team.Create(
            organization.Id,
            $"Time {Guid.NewGuid():N}");
        var player = Player.Create(
            team.Id,
            $"Jogador {Guid.NewGuid():N}");

        await using (var dbContext = await CreateDbContextAsync())
        {
            await dbContext.Organizations.AddAsync(organization);
            await dbContext.Teams.AddAsync(team);
            await dbContext.SaveChangesAsync();

            var repository = new PlayerRepository(dbContext);

            await repository.AddAsync(player);
        }

        await using var verificationContext =
            await CreateDbContextAsync();

        var persistedPlayer = await verificationContext
            .Players
            .AsNoTracking()
            .SingleAsync(item => item.Id == player.Id);

        Assert.Equal(player.Id, persistedPlayer.Id);
        Assert.Equal(team.Id, persistedPlayer.TeamId);
        Assert.Equal(player.Name, persistedPlayer.Name);
    }

    [Fact]
    public async Task ExistsByNameAsync_ShouldScopeNameToTeam()
    {
        var organization = Organization.Create(
            $"Liga {Guid.NewGuid():N}");
        var firstTeam = Team.Create(
            organization.Id,
            $"Time A {Guid.NewGuid():N}");
        var secondTeam = Team.Create(
            organization.Id,
            $"Time B {Guid.NewGuid():N}");
        var playerName = $"Jogador {Guid.NewGuid():N}";
        var player = Player.Create(
            firstTeam.Id,
            playerName);

        await using var dbContext = await CreateDbContextAsync();

        await dbContext.Organizations.AddAsync(organization);
        await dbContext.Teams.AddRangeAsync(
            firstTeam,
            secondTeam);
        await dbContext.SaveChangesAsync();

        var repository = new PlayerRepository(dbContext);

        await repository.AddAsync(player);

        var foundInFirstTeam =
            await repository.ExistsByNameAsync(
                firstTeam.Id,
                playerName);

        var foundInSecondTeam =
            await repository.ExistsByNameAsync(
                secondTeam.Id,
                playerName);

        Assert.True(foundInFirstTeam);
        Assert.False(foundInSecondTeam);
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
