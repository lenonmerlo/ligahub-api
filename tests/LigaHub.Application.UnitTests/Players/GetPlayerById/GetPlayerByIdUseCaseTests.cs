using LigaHub.Application.Players.GetPlayerById;
using LigaHub.Domain.Players;
using LigaHub.Domain.Teams;

namespace LigaHub.Application.UnitTests.Players.GetPlayerById;

public sealed class GetPlayerByIdUseCaseTests
{
    [Fact]
    public async Task Execute_ShouldReturnPlayer_WhenPlayerExists()
    {
        var organizationId = Guid.NewGuid();
        var team = Team.Create(
            organizationId,
            "Time Regional");
        var player = Player.Create(
            team.Id,
            "Jogador Regional",
            new DateOnly(2000, 1, 1),
            Sex.Male,
            10);
        var teamRepository = new FakeTeamRepository
        {
            TeamToReturn = team
        };
        var playerRepository = new FakePlayerRepository
        {
            PlayerToReturn = player
        };
        var useCase = new GetPlayerByIdUseCase(
            teamRepository,
            playerRepository);
        var query = new GetPlayerByIdQuery(
            organizationId,
            team.Id,
            player.Id);

        var result = await useCase.ExecuteAsync(query);

        Assert.NotNull(result);
        Assert.Equal(player.Id, result.Id);
        Assert.Equal(team.Id, result.TeamId);
        Assert.Equal(player.Name, result.Name);
        Assert.Equal(player.BirthDate, result.BirthDate);
        Assert.Equal(player.Sex, result.Sex);
        Assert.Equal(player.JerseyNumber, result.JerseyNumber);
        Assert.Equal(
            organizationId,
            teamRepository.RequestedOrganizationId);
        Assert.Equal(team.Id, teamRepository.RequestedId);
        Assert.Equal(team.Id, playerRepository.RequestedTeamId);
        Assert.Equal(player.Id, playerRepository.RequestedId);
        Assert.Equal(1, teamRepository.GetByIdCalls);
        Assert.Equal(1, playerRepository.GetByIdCalls);
    }

    [Fact]
    public async Task Execute_ShouldReturnNull_WhenTeamDoesNotExistInOrganization()
    {
        var teamRepository = new FakeTeamRepository();
        var playerRepository = new FakePlayerRepository();
        var useCase = new GetPlayerByIdUseCase(
            teamRepository,
            playerRepository);
        var query = new GetPlayerByIdQuery(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid());

        var result = await useCase.ExecuteAsync(query);

        Assert.Null(result);
        Assert.Equal(
            query.OrganizationId,
            teamRepository.RequestedOrganizationId);
        Assert.Equal(query.TeamId, teamRepository.RequestedId);
        Assert.Equal(1, teamRepository.GetByIdCalls);
        Assert.Equal(0, playerRepository.GetByIdCalls);
    }

    [Fact]
    public async Task Execute_ShouldReturnNull_WhenPlayerDoesNotExistInTeam()
    {
        var organizationId = Guid.NewGuid();
        var team = Team.Create(
            organizationId,
            "Time Regional");
        var teamRepository = new FakeTeamRepository
        {
            TeamToReturn = team
        };
        var playerRepository = new FakePlayerRepository();
        var useCase = new GetPlayerByIdUseCase(
            teamRepository,
            playerRepository);
        var query = new GetPlayerByIdQuery(
            organizationId,
            team.Id,
            Guid.NewGuid());

        var result = await useCase.ExecuteAsync(query);

        Assert.Null(result);
        Assert.Equal(team.Id, playerRepository.RequestedTeamId);
        Assert.Equal(query.Id, playerRepository.RequestedId);
        Assert.Equal(1, teamRepository.GetByIdCalls);
        Assert.Equal(1, playerRepository.GetByIdCalls);
    }

    [Fact]
    public async Task Execute_ShouldThrowArgumentNullException_WhenQueryIsNull()
    {
        var teamRepository = new FakeTeamRepository();
        var playerRepository = new FakePlayerRepository();
        var useCase = new GetPlayerByIdUseCase(
            teamRepository,
            playerRepository);

        var exception = await Assert.ThrowsAsync<ArgumentNullException>(
            () => useCase.ExecuteAsync(null!));

        Assert.Equal("query", exception.ParamName);
        Assert.Equal(0, teamRepository.GetByIdCalls);
        Assert.Equal(0, playerRepository.GetByIdCalls);
    }
}
