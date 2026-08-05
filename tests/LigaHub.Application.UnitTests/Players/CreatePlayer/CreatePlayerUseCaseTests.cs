using LigaHub.Application.Players;
using LigaHub.Application.Players.CreatePlayer;
using LigaHub.Domain.Players;
using LigaHub.Domain.Teams;

namespace LigaHub.Application.UnitTests.Players.CreatePlayer;

public sealed class CreatePlayerUseCaseTests
{
    private static readonly DateOnly ValidBirthDate =
        new(2000, 1, 1);

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
            "  Jogador Regional  ",
            ValidBirthDate,
            Sex.Male,
            10);

        var result = await useCase.ExecuteAsync(command);

        Assert.NotNull(result);
        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.Equal(team.Id, result.TeamId);
        Assert.Equal("Jogador Regional", result.Name);
        Assert.Equal(ValidBirthDate, result.BirthDate);
        Assert.Equal(Sex.Male, result.Sex);
        Assert.Equal(10, result.JerseyNumber);
        Assert.Equal(
            organizationId,
            teamRepository.RequestedOrganizationId);
        Assert.Equal(team.Id, teamRepository.RequestedId);
        Assert.Equal(team.Id, playerRepository.RequestedTeamId);
        Assert.Equal(
            10,
            playerRepository.RequestedJerseyNumber);
        Assert.NotNull(playerRepository.AddedPlayer);
        Assert.Equal(
            result.Id,
            playerRepository.AddedPlayer?.Id);
        Assert.Equal(
            ValidBirthDate,
            playerRepository.AddedPlayer?.BirthDate);
        Assert.Equal(
            Sex.Male,
            playerRepository.AddedPlayer?.Sex);
        Assert.Equal(
            10,
            playerRepository.AddedPlayer?.JerseyNumber);
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
            "Jogador Regional",
            ValidBirthDate,
            Sex.Female,
            7);

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
    public async Task Execute_ShouldThrowException_WhenJerseyNumberAlreadyExists()
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
            JerseyNumberExists = true
        };
        var useCase = new CreatePlayerUseCase(
            teamRepository,
            playerRepository);
        var command = new CreatePlayerCommand(
            organizationId,
            team.Id,
            "Jogador Regional",
            ValidBirthDate,
            Sex.Male,
            10);

        var exception =
            await Assert.ThrowsAsync<
                PlayerJerseyNumberAlreadyExistsException>(
                () => useCase.ExecuteAsync(command));

        Assert.Contains("10", exception.Message);
        Assert.Equal(team.Id, playerRepository.RequestedTeamId);
        Assert.Equal(
            10,
            playerRepository.RequestedJerseyNumber);
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
