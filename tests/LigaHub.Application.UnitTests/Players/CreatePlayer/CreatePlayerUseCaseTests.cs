using LigaHub.Application.Players;
using LigaHub.Application.Players.CreatePlayer;
using LigaHub.Domain.Teams;

namespace LigaHub.Application.UnitTests.Players.CreatePlayer;

public sealed class CreatePlayerUseCaseTests
{
    [Fact]
    public async Task Execute_ShouldCreatePlayer_WhenRequestIsValid()
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
        var useCase = new CreatePlayerUseCase(
            teamRepository,
            playerRepository);
        var command = new CreatePlayerCommand(
            organizationId,
            team.Id,
            "  Jogador Regional  ");

        var result = await useCase.ExecuteAsync(command);

        Assert.NotNull(result);
        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.Equal(team.Id, result.TeamId);
        Assert.Equal("Jogador Regional", result.Name);
        Assert.Equal(
            organizationId,
            teamRepository.RequestedOrganizationId);
        Assert.Equal(team.Id, teamRepository.RequestedId);
        Assert.Equal(team.Id, playerRepository.RequestedTeamId);
        Assert.Equal(
            "Jogador Regional",
            playerRepository.RequestedName);
        Assert.NotNull(playerRepository.AddedPlayer);
        Assert.Equal(
            result.Id,
            playerRepository.AddedPlayer?.Id);
        Assert.Equal(1, playerRepository.ExistsCalls);
        Assert.Equal(1, playerRepository.AddCalls);
    }

    [Fact]
    public async Task Execute_ShouldReturnNull_WhenTeamDoesNotExist()
    {
        var teamRepository = new FakeTeamRepository();
        var playerRepository = new FakePlayerRepository();
        var useCase = new CreatePlayerUseCase(
            teamRepository,
            playerRepository);
        var command = new CreatePlayerCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Jogador Regional");

        var result = await useCase.ExecuteAsync(command);

        Assert.Null(result);
        Assert.Equal(
            command.OrganizationId,
            teamRepository.RequestedOrganizationId);
        Assert.Equal(command.TeamId, teamRepository.RequestedId);
        Assert.Equal(0, playerRepository.ExistsCalls);
        Assert.Equal(0, playerRepository.AddCalls);
        Assert.Null(playerRepository.AddedPlayer);
    }

    [Fact]
    public async Task Execute_ShouldThrowException_WhenNameAlreadyExists()
    {
        var organizationId = Guid.NewGuid();
        var team = Team.Create(
            organizationId,
            "Time Regional");
        var teamRepository = new FakeTeamRepository
        {
            TeamToReturn = team
        };
        var playerRepository = new FakePlayerRepository
        {
            NameExists = true
        };
        var useCase = new CreatePlayerUseCase(
            teamRepository,
            playerRepository);
        var command = new CreatePlayerCommand(
            organizationId,
            team.Id,
            "Jogador Regional");

        var exception =
            await Assert.ThrowsAsync<PlayerNameAlreadyExistsException>(
                () => useCase.ExecuteAsync(command));

        Assert.Contains("Jogador Regional", exception.Message);
        Assert.Equal(1, playerRepository.ExistsCalls);
        Assert.Equal(0, playerRepository.AddCalls);
        Assert.Null(playerRepository.AddedPlayer);
    }

    [Fact]
    public async Task Execute_ShouldThrowArgumentNullException_WhenCommandIsNull()
    {
        var teamRepository = new FakeTeamRepository();
        var playerRepository = new FakePlayerRepository();
        var useCase = new CreatePlayerUseCase(
            teamRepository,
            playerRepository);

        var exception = await Assert.ThrowsAsync<ArgumentNullException>(
            () => useCase.ExecuteAsync(null!));

        Assert.Equal("command", exception.ParamName);
    }
}
