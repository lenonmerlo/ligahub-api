using LigaHub.Application.Teams.DeleteTeam;
using LigaHub.Domain.Teams;

namespace LigaHub.Application.UnitTests.Teams.DeleteTeam;

public sealed class DeleteTeamUseCaseTests
{
    [Fact]
    public async Task Execute_ShouldDeleteTeam_WhenTeamExists()
    {
        var organizationId = Guid.NewGuid();
        var team = Team.Create(
            organizationId,
            "Time Regional");
        var repository = new FakeTeamRepository
        {
            TeamToReturn = team
        };
        var useCase = new DeleteTeamUseCase(repository);
        var command = new DeleteTeamCommand(
            organizationId,
            team.Id);

        var result = await useCase.ExecuteAsync(command);

        Assert.True(result);
        Assert.Equal(
            organizationId,
            repository.RequestedOrganizationId);
        Assert.Equal(team.Id, repository.RequestedId);
        Assert.Same(team, repository.DeletedTeam);
        Assert.Equal(1, repository.DeleteCalls);
    }

    [Fact]
    public async Task Execute_ShouldReturnFalse_WhenTeamDoesNotExist()
    {
        var repository = new FakeTeamRepository();
        var useCase = new DeleteTeamUseCase(repository);
        var command = new DeleteTeamCommand(
            Guid.NewGuid(),
            Guid.NewGuid());

        var result = await useCase.ExecuteAsync(command);

        Assert.False(result);
        Assert.Equal(
            command.OrganizationId,
            repository.RequestedOrganizationId);
        Assert.Equal(command.Id, repository.RequestedId);
        Assert.Null(repository.DeletedTeam);
        Assert.Equal(0, repository.DeleteCalls);
    }

    [Fact]
    public async Task Execute_ShouldThrowArgumentNullException_WhenCommandIsNull()
    {
        var repository = new FakeTeamRepository();
        var useCase = new DeleteTeamUseCase(repository);

        var exception = await Assert.ThrowsAsync<ArgumentNullException>(
            () => useCase.ExecuteAsync(null!));

        Assert.Equal("command", exception.ParamName);
    }
}
