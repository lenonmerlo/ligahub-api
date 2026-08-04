using LigaHub.Application.Teams;
using LigaHub.Application.Teams.CreateTeam;
using LigaHub.Domain.Organizations;
using LigaHub.Domain.Teams;

namespace LigaHub.Application.UnitTests.Teams.CreateTeam;

public sealed class CreateTeamUseCaseTests
{
    [Fact]
    public async Task Execute_ShouldCreateTeam_WhenRequestIsValid()
    {
        var organization = Organization.Create("Liga Regional");
        var organizationRepository = new FakeOrganizationRepository
        {
            OrganizationToReturn = organization
        };
        var teamRepository = new FakeTeamRepository();
        var useCase = new CreateTeamUseCase(
            organizationRepository,
            teamRepository);
        var command = new CreateTeamCommand(
            organization.Id,
            "  Time Regional  ",
            Sport.Basketball);

        var result = await useCase.ExecuteAsync(command);

        Assert.NotNull(result);
        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.Equal(organization.Id, result.OrganizationId);
        Assert.Equal("Time Regional", result.Name);
        Assert.Equal(Sport.Basketball, result.Sport);
        Assert.Equal(
            Sport.Basketball,
            teamRepository.AddedTeam?.Sport);
        Assert.Equal(organization.Id, organizationRepository.RequestedId);
        Assert.Equal(organization.Id, teamRepository.RequestedOrganizationId);
        Assert.Equal("Time Regional", teamRepository.RequestedName);
        Assert.NotNull(teamRepository.AddedTeam);
        Assert.Equal(result.Id, teamRepository.AddedTeam?.Id);
        Assert.Equal(1, teamRepository.ExistsCalls);
        Assert.Equal(1, teamRepository.AddCalls);
    }

    [Fact]
    public async Task Execute_ShouldReturnNull_WhenOrganizationDoesNotExist()
    {
        var organizationRepository = new FakeOrganizationRepository();
        var teamRepository = new FakeTeamRepository();
        var useCase = new CreateTeamUseCase(
            organizationRepository,
            teamRepository);
        var command = new CreateTeamCommand(
            Guid.NewGuid(),
            "Time Regional");

        var result = await useCase.ExecuteAsync(command);

        Assert.Null(result);
        Assert.Equal(command.OrganizationId, organizationRepository.RequestedId);
        Assert.Equal(0, teamRepository.ExistsCalls);
        Assert.Equal(0, teamRepository.AddCalls);
        Assert.Null(teamRepository.AddedTeam);
    }

    [Fact]
    public async Task Execute_ShouldThrowException_WhenNameAlreadyExists()
    {
        var organization = Organization.Create("Liga Regional");
        var organizationRepository = new FakeOrganizationRepository
        {
            OrganizationToReturn = organization
        };
        var teamRepository = new FakeTeamRepository
        {
            NameExists = true
        };
        var useCase = new CreateTeamUseCase(
            organizationRepository,
            teamRepository);
        var command = new CreateTeamCommand(
            organization.Id,
            "Time Regional");

        var exception =
            await Assert.ThrowsAsync<TeamNameAlreadyExistsException>(
                () => useCase.ExecuteAsync(command));

        Assert.Contains("Time Regional", exception.Message);
        Assert.Equal(1, teamRepository.ExistsCalls);
        Assert.Equal(0, teamRepository.AddCalls);
        Assert.Null(teamRepository.AddedTeam);
    }

    [Fact]
    public async Task Execute_ShouldThrowArgumentNullException_WhenCommandIsNull()
    {
        var organizationRepository = new FakeOrganizationRepository();
        var teamRepository = new FakeTeamRepository();
        var useCase = new CreateTeamUseCase(
            organizationRepository,
            teamRepository);

        var exception = await Assert.ThrowsAsync<ArgumentNullException>(
            () => useCase.ExecuteAsync(null!));

        Assert.Equal("command", exception.ParamName);
    }
}
