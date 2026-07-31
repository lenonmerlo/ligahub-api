using LigaHub.Application.Teams.ListTeams;
using LigaHub.Domain.Organizations;
using LigaHub.Domain.Teams;

namespace LigaHub.Application.UnitTests.Teams.ListTeams;

public sealed class ListTeamsUseCaseTests
{
    [Fact]
    public async Task Execute_ShouldReturnPaginatedTeams()
    {
        var organization = Organization.Create("Liga Regional");
        var firstTeam = Team.Create(organization.Id, "Time A");
        var secondTeam = Team.Create(organization.Id, "Time B");
        var organizationRepository = new FakeOrganizationRepository
        {
            OrganizationToReturn = organization
        };
        var teamRepository = new FakeTeamRepository
        {
            Teams =
            [
                firstTeam,
                secondTeam
            ],
            TotalCount = 5
        };
        var useCase = new ListTeamsUseCase(
            organizationRepository,
            teamRepository);
        var query = new ListTeamsQuery(
            organization.Id,
            Page: 2,
            PageSize: 2);

        var result = await useCase.ExecuteAsync(query);

        Assert.NotNull(result);
        Assert.Equal(2, result.Page);
        Assert.Equal(2, result.PageSize);
        Assert.Equal(5, result.TotalCount);
        Assert.Equal(3, result.TotalPages);
        Assert.Equal(organization.Id, organizationRepository.RequestedId);
        Assert.Equal(
            organization.Id,
            teamRepository.RequestedListOrganizationId);
        Assert.Equal(
            organization.Id,
            teamRepository.RequestedCountOrganizationId);
        Assert.Equal(2, teamRepository.RequestedSkip);
        Assert.Equal(2, teamRepository.RequestedTake);
        Assert.Collection(
            result.Items,
            item =>
            {
                Assert.Equal(firstTeam.Id, item.Id);
                Assert.Equal(firstTeam.Name, item.Name);
            },
            item =>
            {
                Assert.Equal(secondTeam.Id, item.Id);
                Assert.Equal(secondTeam.Name, item.Name);
            });
    }

    [Fact]
    public async Task Execute_ShouldReturnNull_WhenOrganizationDoesNotExist()
    {
        var organizationRepository = new FakeOrganizationRepository();
        var teamRepository = new FakeTeamRepository();
        var useCase = new ListTeamsUseCase(
            organizationRepository,
            teamRepository);
        var query = new ListTeamsQuery(Guid.NewGuid());

        var result = await useCase.ExecuteAsync(query);

        Assert.Null(result);
        Assert.Equal(query.OrganizationId, organizationRepository.RequestedId);
        Assert.Null(teamRepository.RequestedListOrganizationId);
        Assert.Null(teamRepository.RequestedCountOrganizationId);
    }

    [Fact]
    public async Task Execute_ShouldThrowArgumentException_WhenOrganizationIdIsEmpty()
    {
        var organizationRepository = new FakeOrganizationRepository();
        var teamRepository = new FakeTeamRepository();
        var useCase = new ListTeamsUseCase(
            organizationRepository,
            teamRepository);
        var query = new ListTeamsQuery(Guid.Empty);

        var exception = await Assert.ThrowsAsync<ArgumentException>(
            () => useCase.ExecuteAsync(query));

        Assert.Equal("OrganizationId", exception.ParamName);
    }

    [Fact]
    public async Task Execute_ShouldThrowArgumentOutOfRangeException_WhenPageIsInvalid()
    {
        var useCase = CreateUseCase();
        var query = new ListTeamsQuery(
            Guid.NewGuid(),
            Page: 0);

        var exception = await Assert.ThrowsAsync<ArgumentOutOfRangeException>(
            () => useCase.ExecuteAsync(query));

        Assert.Equal("Page", exception.ParamName);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(ListTeamsUseCase.MaxPageSize + 1)]
    public async Task Execute_ShouldThrowArgumentOutOfRangeException_WhenPageSizeIsInvalid(
        int pageSize)
    {
        var useCase = CreateUseCase();
        var query = new ListTeamsQuery(
            Guid.NewGuid(),
            PageSize: pageSize);

        var exception = await Assert.ThrowsAsync<ArgumentOutOfRangeException>(
            () => useCase.ExecuteAsync(query));

        Assert.Equal("PageSize", exception.ParamName);
    }

    [Fact]
    public async Task Execute_ShouldThrowArgumentOutOfRangeException_WhenPageIsTooLarge()
    {
        var useCase = CreateUseCase();
        var query = new ListTeamsQuery(
            Guid.NewGuid(),
            Page: int.MaxValue,
            PageSize: 2);

        var exception = await Assert.ThrowsAsync<ArgumentOutOfRangeException>(
            () => useCase.ExecuteAsync(query));

        Assert.Equal("Page", exception.ParamName);
    }

    [Fact]
    public async Task Execute_ShouldThrowArgumentNullException_WhenQueryIsNull()
    {
        var useCase = CreateUseCase();

        var exception = await Assert.ThrowsAsync<ArgumentNullException>(
            () => useCase.ExecuteAsync(null!));

        Assert.Equal("query", exception.ParamName);
    }

    private static ListTeamsUseCase CreateUseCase()
    {
        return new ListTeamsUseCase(
            new FakeOrganizationRepository(),
            new FakeTeamRepository());
    }
}
