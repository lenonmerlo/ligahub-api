using LigaHub.Application.Teams.GetTeamById;
using LigaHub.Domain.Teams;

namespace LigaHub.Application.UnitTests.Teams.GetTeamById;

public sealed class GetTeamByIdUseCaseTests
{
    [Fact]
    public async Task Execute_ShouldReturnTeam_WhenTeamExists()
    {
        var organizationId = Guid.NewGuid();
        var team = Team.Create(
            organizationId,
            "Time Regional");
        var repository = new FakeTeamRepository
        {
            TeamToReturn = team
        };
        var useCase = new GetTeamByIdUseCase(repository);
        var query = new GetTeamByIdQuery(
            organizationId,
            team.Id);

        var result = await useCase.ExecuteAsync(query);

        Assert.NotNull(result);
        Assert.Equal(team.Id, result.Id);
        Assert.Equal(organizationId, result.OrganizationId);
        Assert.Equal(team.Name, result.Name);
        Assert.Equal(
            organizationId,
            repository.RequestedOrganizationId);
        Assert.Equal(team.Id, repository.RequestedId);
    }

    [Fact]
    public async Task Execute_ShouldReturnNull_WhenTeamDoesNotExist()
    {
        var repository = new FakeTeamRepository();
        var useCase = new GetTeamByIdUseCase(repository);
        var query = new GetTeamByIdQuery(
            Guid.NewGuid(),
            Guid.NewGuid());

        var result = await useCase.ExecuteAsync(query);

        Assert.Null(result);
        Assert.Equal(
            query.OrganizationId,
            repository.RequestedOrganizationId);
        Assert.Equal(query.Id, repository.RequestedId);
    }

    [Fact]
    public async Task Execute_ShouldThrowArgumentNullException_WhenQueryIsNull()
    {
        var repository = new FakeTeamRepository();
        var useCase = new GetTeamByIdUseCase(repository);

        var exception = await Assert.ThrowsAsync<ArgumentNullException>(
            () => useCase.ExecuteAsync(null!));

        Assert.Equal("query", exception.ParamName);
    }
}
