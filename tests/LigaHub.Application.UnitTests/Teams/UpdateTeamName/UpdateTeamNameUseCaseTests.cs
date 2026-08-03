using LigaHub.Application.Teams;
using LigaHub.Application.Teams.UpdateTeamName;
using LigaHub.Domain.Teams;

namespace LigaHub.Application.UnitTests.Teams.UpdateTeamName;

public sealed class UpdateTeamNameUseCaseTests
{
    [Fact]
    public async Task Execute_ShouldUpdateTeamName()
    {
        var organizationId = Guid.NewGuid();
        var team = Team.Create(
            organizationId,
            "Old Name");
        var repository = new FakeTeamRepository
        {
            TeamToReturn = team
        };
        var useCase = new UpdateTeamNameUseCase(repository);
        var command = new UpdateTeamNameCommand(
            organizationId,
            team.Id,
            "  New Name  ");

        var result = await useCase.ExecuteAsync(command);

        Assert.NotNull(result);
        Assert.Equal(team.Id, result.Id);
        Assert.Equal(organizationId, result.OrganizationId);
        Assert.Equal("New Name", result.Name);
        Assert.Equal(organizationId, repository.RequestedOrganizationId);
        Assert.Equal(team.Id, repository.RequestedId);
        Assert.Equal(organizationId, repository.NameCheckOrganizationId);
        Assert.Equal("New Name", repository.RequestedName);
        Assert.Same(team, repository.UpdatedTeam);
        Assert.Equal(1, repository.UpdateCalls);
        Assert.Equal(1, repository.ExistsByNameCalls);
    }

    [Fact]
    public async Task Execute_ShouldReturnNull_WhenTeamDoesNotExist()
    {
        var repository = new FakeTeamRepository();
        var useCase = new UpdateTeamNameUseCase(repository);
        var command = new UpdateTeamNameCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "New Name");

        var result = await useCase.ExecuteAsync(command);

        Assert.Null(result);
        Assert.Equal(
            command.OrganizationId,
            repository.RequestedOrganizationId);
        Assert.Equal(command.Id, repository.RequestedId);
        Assert.Equal(0, repository.UpdateCalls);
    }

    [Fact]
    public async Task Execute_ShouldThrowConflict_WhenNameAlreadyExists()
    {
        var organizationId = Guid.NewGuid();
        var team = Team.Create(
            organizationId,
            "Old Name");
        var repository = new FakeTeamRepository
        {
            TeamToReturn = team,
            NameExists = true
        };
        var useCase = new UpdateTeamNameUseCase(repository);
        var command = new UpdateTeamNameCommand(
            organizationId,
            team.Id,
            "Existing Name");

        await Assert.ThrowsAsync<TeamNameAlreadyExistsException>(
            () => useCase.ExecuteAsync(command));

        Assert.Equal(organizationId, repository.NameCheckOrganizationId);
        Assert.Equal(0, repository.UpdateCalls);
    }

    [Fact]
    public async Task Execute_ShouldAllowNameChangeWithDifferentCasing()
    {
        var organizationId = Guid.NewGuid();
        var team = Team.Create(
            organizationId,
            "Time Regional");
        var repository = new FakeTeamRepository
        {
            TeamToReturn = team,
            NameExists = true
        };
        var useCase = new UpdateTeamNameUseCase(repository);
        var command = new UpdateTeamNameCommand(
            organizationId,
            team.Id,
            "time regional");

        var result = await useCase.ExecuteAsync(command);

        Assert.NotNull(result);
        Assert.Equal("time regional", result.Name);
        Assert.Equal(0, repository.ExistsByNameCalls);
        Assert.Equal(1, repository.UpdateCalls);
    }

    [Fact]
    public async Task Execute_ShouldThrowArgumentException_WhenNameIsInvalid()
    {
        var organizationId = Guid.NewGuid();
        var team = Team.Create(
            organizationId,
            "Old Name");
        var repository = new FakeTeamRepository
        {
            TeamToReturn = team
        };
        var useCase = new UpdateTeamNameUseCase(repository);
        var command = new UpdateTeamNameCommand(
            organizationId,
            team.Id,
            " ");

        var exception = await Assert.ThrowsAsync<ArgumentException>(
            () => useCase.ExecuteAsync(command));

        Assert.Equal("name", exception.ParamName);
        Assert.Equal(0, repository.UpdateCalls);
    }

    [Fact]
    public async Task Execute_ShouldThrowArgumentNullException_WhenCommandIsNull()
    {
        var repository = new FakeTeamRepository();
        var useCase = new UpdateTeamNameUseCase(repository);

        var exception = await Assert.ThrowsAsync<ArgumentNullException>(
            () => useCase.ExecuteAsync(null!));

        Assert.Equal("command", exception.ParamName);
    }
}
