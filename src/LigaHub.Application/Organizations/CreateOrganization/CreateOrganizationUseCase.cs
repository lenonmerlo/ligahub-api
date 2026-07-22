using LigaHub.Domain.Organizations;

namespace LigaHub.Application.Organizations.CreateOrganization;

public sealed class CreateOrganizationUseCase
{
    private readonly IOrganizationRepository _repository;

    public CreateOrganizationUseCase(IOrganizationRepository repository)
    {
        _repository = repository
            ?? throw new ArgumentException(nameof(repository));
    }

    public async Task<CreateOrganizationResult> ExecuteAsync(
        CreateOrganizationCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        var organization = Organization.Create(command.Name);

        var nameAlreadyExists = await _repository.ExistsByNameAsync(
            organization.Name,
            cancellationToken);

        if (nameAlreadyExists)
        {
            throw new OrganizationNameAlreadyExistsException(
                organization.Name);
        }

        await _repository.AddAsync(organization, cancellationToken);

        return new CreateOrganizationResult(
            organization.Id,
            organization.Name);
    }
}