using LigaHub.Application.Organizations.ListOrganizations;
using LigaHub.Domain.Organizations;

namespace LigaHub.Application.UnitTests.Organizations.ListOrganizations;

public sealed class ListOrganizationsUseCaseTests
{
    [Fact]
    public async Task Execute_ShouldReturnPaginatedOrganizations()
    {
        var firstOrganization = Organization.Create("Liga A");
        var secondOrganization = Organization.Create("Liga B");

        var repository = new FakeOrganizationRepository
        {
            Organizations =
            [
                firstOrganization,
                secondOrganization
            ],
            TotalCount = 5
        };

        var useCase = new ListOrganizationsUseCase(repository);
        var query = new ListOrganizationsQuery(Page: 2, PageSize: 2);

        var result = await useCase.ExecuteAsync(query);

        Assert.Equal(2, result.Page);
        Assert.Equal(2, result.PageSize);
        Assert.Equal(5, result.TotalCount);
        Assert.Equal(3, result.TotalPages);
        Assert.Equal(2, repository.RequestedSkip);
        Assert.Equal(2, repository.RequestedTake);
        Assert.Collection(
            result.Items,
            item =>
            {
                Assert.Equal(firstOrganization.Id, item.Id);
                Assert.Equal(firstOrganization.Name, item.Name);
            },
            item =>
            {
                Assert.Equal(secondOrganization.Id, item.Id);
                Assert.Equal(secondOrganization.Name, item.Name);
            });
    }

    [Fact]
    public async Task Execute_ShouldThrowArgumentOutOfRangeException_WhenPageIsInvalid()
    {
        var repository = new FakeOrganizationRepository();
        var useCase = new ListOrganizationsUseCase(repository);
        var query = new ListOrganizationsQuery(Page: 0);

        var exception = await Assert.ThrowsAsync<ArgumentOutOfRangeException>(
            () => useCase.ExecuteAsync(query));

        Assert.Equal("Page", exception.ParamName);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(ListOrganizationsUseCase.MaxPageSize + 1)]
    public async Task Execute_ShouldThrowArgumentOutOfRangeException_WhenPageSizeIsInvalid(
        int pageSize)
    {
        var repository = new FakeOrganizationRepository();
        var useCase = new ListOrganizationsUseCase(repository);
        var query = new ListOrganizationsQuery(PageSize: pageSize);

        var exception = await Assert.ThrowsAsync<ArgumentOutOfRangeException>(
            () => useCase.ExecuteAsync(query));

        Assert.Equal("PageSize", exception.ParamName);
    }

    [Fact]
    public async Task Execute_ShouldThrowArgumentOutOfRangeException_WhenPageIsTooLarge()
    {
        var repository = new FakeOrganizationRepository();
        var useCase = new ListOrganizationsUseCase(repository);
        var query = new ListOrganizationsQuery(
            Page: int.MaxValue,
            PageSize: 2);

        var exception = await Assert.ThrowsAsync<ArgumentOutOfRangeException>(
            () => useCase.ExecuteAsync(query));

        Assert.Equal("Page", exception.ParamName);
    }

    [Fact]
    public async Task Execute_ShouldThrowArgumentNullException_WhenQueryIsNull()
    {
        var repository = new FakeOrganizationRepository();
        var useCase = new ListOrganizationsUseCase(repository);

        var exception = await Assert.ThrowsAsync<ArgumentNullException>(
            () => useCase.ExecuteAsync(null!));

        Assert.Equal("query", exception.ParamName);
    }
}
