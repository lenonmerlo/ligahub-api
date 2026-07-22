using LigaHub.Application.Organizations;
using LigaHub.Application.Organizations.CreateOrganization;

namespace LigaHub.Application.UnitTests.Organizations.CreateOrganization;

public sealed class CreateOrganizationUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_ShouldCreateOrganization_WhenNameIsAvailable()
    {
        var repository = new FakeOrganizationRepository();
        var useCase = new CreateOrganizationUseCase(repository);
        var command = new CreateOrganizationCommand("  Liga Regional  ");

        var result = await useCase.ExecuteAsync(command);

        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.Equal("Liga Regional", result.Name);
        Assert.Equal(1, repository.AddCalls);
        Assert.NotNull(repository.AddedOrganization);
        Assert.Equal(result.Id, repository.AddedOrganization.Id);
        Assert.Equal(result.Name, repository.AddedOrganization.Name);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldThrowConflict_WhenNameAlreadyExists()
    {
        var repository = new FakeOrganizationRepository
        {
            NameExists = true
        };
        var useCase = new CreateOrganizationUseCase(repository);
        var command = new CreateOrganizationCommand("Liga Regional");

        await Assert.ThrowsAsync<OrganizationNameAlreadyExistsException>(
            () => useCase.ExecuteAsync(command));

        Assert.Equal(0, repository.AddCalls);
        Assert.Null(repository.AddedOrganization);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldNotPersistOrganization_WhenNameIsInvalid()
    {
        var repository = new FakeOrganizationRepository();
        var useCase = new CreateOrganizationUseCase(repository);
        var command = new CreateOrganizationCommand(" ");

        await Assert.ThrowsAsync<ArgumentException>(
            () => useCase.ExecuteAsync(command));

        Assert.Equal(0, repository.AddCalls);
        Assert.Null(repository.AddedOrganization);
    }
}