using LigaHub.Application.Organizations.GetOrganizationById;
using LigaHub.Domain.Organizations;

namespace LigaHub.Application.UnitTests.Organizations.GetOrganizationById;

public sealed class GetOrganizationByIdUseCaseTests
{
    [Fact]
    public async Task Execute_ShouldReturnOrganization_WhenOrganizationExists()
    {
        var organization = Organization.Create("Liga Regional");
        var repository = new FakeOrganizationRepository
        {
            OrganizationToReturn = organization
        };
        var useCase = new GetOrganizationByIdUseCase(repository);
        var query = new GetOrganizationByIdQuery(organization.Id);

        var result = await useCase.ExecuteAsync(query);

        Assert.NotNull(result);
        Assert.Equal(organization.Id, result.Id);
        Assert.Equal(organization.Name, result.Name);
        Assert.Equal(organization.Id, repository.RequestedId);
    }

    [Fact]
    public async Task Execute_ShouldReturnNull_WhenOrganizationDoesNotExist()
    {
        var repository = new FakeOrganizationRepository();
        var useCase = new GetOrganizationByIdUseCase(repository);
        var query = new GetOrganizationByIdQuery(Guid.NewGuid());

        var result = await useCase.ExecuteAsync(query);

        Assert.Null(result);
        Assert.Equal(query.Id, repository.RequestedId);
    }

    [Fact]
    public async Task Execute_ShouldThrowArgumentNullException_WhenQueryIsNull()
    {
        var repository = new FakeOrganizationRepository();
        var useCase = new GetOrganizationByIdUseCase(repository);

        var exception = await Assert.ThrowsAsync<ArgumentNullException>(
            () => useCase.ExecuteAsync(null!));

        Assert.Equal("query", exception.ParamName);
    }
}
