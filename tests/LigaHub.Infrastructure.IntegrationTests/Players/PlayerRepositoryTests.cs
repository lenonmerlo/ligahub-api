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
    public async Task GetByIdAsync_ShouldScopePlayerToTeam()
    {
        var organization = Organization.Create(
            $"Liga {Guid.NewGuid():N}");
        var firstTeam = Team.Create(
            organization.Id,
            $"Time A {Guid.NewGuid():N}");
        var secondTeam = Team.Create(
            organization.Id,
            $"Time B {Guid.NewGuid():N}");
        var player = Player.Create(
            firstTeam.Id,
            $"Jogador {Guid.NewGuid():N}",
            new DateOnly(2000, 1, 1),
            Sex.Male,
            10);

        await using var dbContext = await CreateDbContextAsync();

        await dbContext.Organizations.AddAsync(organization);
        await dbContext.Teams.AddRangeAsync(
            firstTeam,
            secondTeam);
        await dbContext.SaveChangesAsync();

        var repository = new PlayerRepository(dbContext);

        await repository.AddAsync(player);

        var foundInFirstTeam = await repository.GetByIdAsync(
            firstTeam.Id,
            player.Id);

        var foundInSecondTeam = await repository.GetByIdAsync(
            secondTeam.Id,
            player.Id);

        Assert.NotNull(foundInFirstTeam);
        Assert.Equal(player.Id, foundInFirstTeam.Id);
        Assert.Equal(firstTeam.Id, foundInFirstTeam.TeamId);
        Assert.Equal(player.Name, foundInFirstTeam.Name);
        Assert.Equal(player.BirthDate, foundInFirstTeam.BirthDate);
        Assert.Equal(player.Sex, foundInFirstTeam.Sex);
        Assert.Equal(
            player.JerseyNumber,
            foundInFirstTeam.JerseyNumber);
        Assert.Null(foundInSecondTeam);
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
            $"Jogador {Guid.NewGuid():N}",
            new DateOnly(2000, 1, 1),
            Sex.Male,
            10);

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
        Assert.Equal(player.BirthDate, persistedPlayer.BirthDate);
        Assert.Equal(player.Sex, persistedPlayer.Sex);
        Assert.Equal(
            player.JerseyNumber,
            persistedPlayer.JerseyNumber);
    }

    [Fact]
    public async Task ExistsByJerseyNumberAsync_ShouldScopeNumberToTeam()
    {
        var organization = Organization.Create(
            $"Liga {Guid.NewGuid():N}");
        var firstTeam = Team.Create(
            organization.Id,
            $"Time A {Guid.NewGuid():N}");
        var secondTeam = Team.Create(
            organization.Id,
            $"Time B {Guid.NewGuid():N}");
        var player = Player.Create(
            firstTeam.Id,
            $"Jogador {Guid.NewGuid():N}",
            new DateOnly(2000, 1, 1),
            Sex.Female,
            7);

        await using var dbContext = await CreateDbContextAsync();

        await dbContext.Organizations.AddAsync(organization);
        await dbContext.Teams.AddRangeAsync(
            firstTeam,
            secondTeam);
        await dbContext.SaveChangesAsync();

        var repository = new PlayerRepository(dbContext);

        await repository.AddAsync(player);

        var foundInFirstTeam =
            await repository.ExistsByJerseyNumberAsync(
                firstTeam.Id,
                player.JerseyNumber);

        var foundInSecondTeam =
            await repository.ExistsByJerseyNumberAsync(
                secondTeam.Id,
                player.JerseyNumber);

        Assert.True(foundInFirstTeam);
        Assert.False(foundInSecondTeam);
    }

    [Fact]
    public async Task AddAsync_ShouldAllowSameNameWithDifferentJerseyNumbers()
    {
        var organization = Organization.Create(
            $"Liga {Guid.NewGuid():N}");
        var team = Team.Create(
            organization.Id,
            $"Time {Guid.NewGuid():N}");
        var playerName = $"Jogador {Guid.NewGuid():N}";
        var firstPlayer = Player.Create(
            team.Id,
            playerName,
            new DateOnly(2000, 1, 1),
            Sex.Male,
            8);
        var secondPlayer = Player.Create(
            team.Id,
            playerName,
            new DateOnly(2001, 2, 2),
            Sex.Female,
            9);

        await using var dbContext = await CreateDbContextAsync();

        await dbContext.Organizations.AddAsync(organization);
        await dbContext.Teams.AddAsync(team);
        await dbContext.SaveChangesAsync();

        var repository = new PlayerRepository(dbContext);

        await repository.AddAsync(firstPlayer);
        await repository.AddAsync(secondPlayer);

        var persistedPlayers = await dbContext.Players
            .AsNoTracking()
            .Where(player => player.TeamId == team.Id)
            .ToArrayAsync();

        Assert.Equal(2, persistedPlayers.Length);
    }

    [Fact]
    public async Task AddAsync_ShouldRejectSameJerseyNumberInSameTeam()
    {
        var organization = Organization.Create(
            $"Liga {Guid.NewGuid():N}");
        var team = Team.Create(
            organization.Id,
            $"Time {Guid.NewGuid():N}");
        var firstPlayer = Player.Create(
            team.Id,
            $"Jogador A {Guid.NewGuid():N}",
            new DateOnly(2000, 1, 1),
            Sex.Male,
            10);
        var secondPlayer = Player.Create(
            team.Id,
            $"Jogador B {Guid.NewGuid():N}",
            new DateOnly(2001, 2, 2),
            Sex.Female,
            10);

        await using var dbContext = await CreateDbContextAsync();

        await dbContext.Organizations.AddAsync(organization);
        await dbContext.Teams.AddAsync(team);
        await dbContext.SaveChangesAsync();

        var repository = new PlayerRepository(dbContext);

        await repository.AddAsync(firstPlayer);

        await Assert.ThrowsAsync<DbUpdateException>(
            () => repository.AddAsync(secondPlayer));
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
