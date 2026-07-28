using LigaHub.Application.Organizations;
using LigaHub.Application.Organizations.UpdateOrganizationName;
using LigaHub.Domain.Organizations;

namespace LigaHub.Application.UnitTests.Organizations.UpdateOrganizationName;

public sealed class UpdateOrganizationNameUseCaseTests
{
    [Fact]
    public async Task Execute_ShouldUpdateOrganizationName()
    {
        var organization = Organization.Create("Old Name");
        var repository = new FakeOrganizationRepository
        {
            OrganizationToReturn = organization
        };
        var useCase = new UpdateOrganizationNameUseCase(repository);
        var command = new UpdateOrganizationNameCommand(
            organization.Id,
            "  New Name  ");

        var result = await useCase.ExecuteAsync(command);

        Assert.NotNull(result);
        Assert.Equal(organization.Id, result.Id);
        Assert.Equal("New Name", result.Name);
        Assert.Same(organization, repository.UpdatedOrganization);
        Assert.Equal(1, repository.UpdateCalls);
        Assert.Equal(1, repository.ExistsByNameCalls);
    }

    [Fact]
    public async Task Execute_ShouldReturnNull_WhenOrganizationDoesNotExist()
    {
        var repository = new FakeOrganizationRepository();
        var useCase = new UpdateOrganizationNameUseCase(repository);
        var command = new UpdateOrganizationNameCommand(
            Guid.NewGuid(),
            "New Name");

        var result = await useCase.ExecuteAsync(command);

        Assert.Null(result);
        Assert.Equal(0, repository.UpdateCalls);
    }

    [Fact]
    public async Task Execute_ShouldThrowConflict_WhenNameAlreadyExists()
    {
        var organization = Organization.Create("Old Name");
        var repository = new FakeOrganizationRepository
        {
            OrganizationToReturn = organization,
            NameExists = true
        };
        var useCase = new UpdateOrganizationNameUseCase(repository);
        var command = new UpdateOrganizationNameCommand(
            organization.Id,
            "Existing Name");

        await Assert.ThrowsAsync<OrganizationNameAlreadyExistsException>(
            () => useCase.ExecuteAsync(command));

        Assert.Equal(0, repository.UpdateCalls);
    }

    [Fact]
    public async Task Execute_ShouldAllowNameChangeWithDifferentCasing()
    {
        var organization = Organization.Create("Liga Regional");
        var repository = new FakeOrganizationRepository
        {
            OrganizationToReturn = organization,
            NameExists = true
        };
        var useCase = new UpdateOrganizationNameUseCase(repository);
        var command = new UpdateOrganizationNameCommand(
            organization.Id,
            "liga regional");

        var result = await useCase.ExecuteAsync(command);

        Assert.NotNull(result);
        Assert.Equal("liga regional", result.Name);
        Assert.Equal(0, repository.ExistsByNameCalls);
        Assert.Equal(1, repository.UpdateCalls);
    }

    [Fact]
    public async Task Execute_ShouldThrowArgumentException_WhenNameIsInvalid()
    {
        var organization = Organization.Create("Old Name");
        var repository = new FakeOrganizationRepository
        {
            OrganizationToReturn = organization
        };
        var useCase = new UpdateOrganizationNameUseCase(repository);
        var command = new UpdateOrganizationNameCommand(
            organization.Id,
            " ");

        var exception = await Assert.ThrowsAsync<ArgumentException>(
            () => useCase.ExecuteAsync(command));

        Assert.Equal("name", exception.ParamName);
        Assert.Equal(0, repository.UpdateCalls);
    }

    [Fact]
    public async Task Execute_ShouldThrowArgumentNullException_WhenCommandIsNull()
    {
        var repository = new FakeOrganizationRepository();
        var useCase = new UpdateOrganizationNameUseCase(repository);

        var exception = await Assert.ThrowsAsync<ArgumentNullException>(
            () => useCase.ExecuteAsync(null!));

        Assert.Equal("command", exception.ParamName);
    }
}
