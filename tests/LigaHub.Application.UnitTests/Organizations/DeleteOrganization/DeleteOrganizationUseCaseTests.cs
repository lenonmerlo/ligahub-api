using LigaHub.Application.Organizations.DeleteOrganization;
using LigaHub.Domain.Organizations;

namespace LigaHub.Application.UnitTests.Organizations.DeleteOrganization;

public sealed class DeleteOrganizationUseCaseTests
{
    [Fact]
    public async Task Execute_ShouldDeleteOrganization_WhenOrganizationExists()
    {
        var organization = Organization.Create("Liga Regional");
        var repository = new FakeOrganizationRepository
        {
            OrganizationToReturn = organization
        };
        var useCase = new DeleteOrganizationUseCase(repository);
        var command = new DeleteOrganizationCommand(organization.Id);

        var result = await useCase.ExecuteAsync(command);

        Assert.True(result);
        Assert.Equal(organization.Id, repository.RequestedId);
        Assert.Same(organization, repository.DeletedOrganization);
        Assert.Equal(1, repository.DeleteCalls);
    }

    [Fact]
    public async Task Execute_ShouldReturnFalse_WhenOrganizationDoesNotExist()
    {
        var repository = new FakeOrganizationRepository();
        var useCase = new DeleteOrganizationUseCase(repository);
        var command = new DeleteOrganizationCommand(Guid.NewGuid());

        var result = await useCase.ExecuteAsync(command);

        Assert.False(result);
        Assert.Equal(command.Id, repository.RequestedId);
        Assert.Null(repository.DeletedOrganization);
        Assert.Equal(0, repository.DeleteCalls);
    }

    [Fact]
    public async Task Execute_ShouldThrowArgumentNullException_WhenCommandIsNull()
    {
        var repository = new FakeOrganizationRepository();
        var useCase = new DeleteOrganizationUseCase(repository);

        var exception = await Assert.ThrowsAsync<ArgumentNullException>(
            () => useCase.ExecuteAsync(null!));

        Assert.Equal("command", exception.ParamName);
    }
}
