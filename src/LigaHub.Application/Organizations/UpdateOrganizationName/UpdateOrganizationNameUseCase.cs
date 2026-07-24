namespace LigaHub.Application.Organizations.UpdateOrganizationName;

public sealed class UpdateOrganizationNameUseCase
{
    private readonly IOrganizationRepository _repository;

    public UpdateOrganizationNameUseCase(
        IOrganizationRepository repository)
    {
        _repository = repository
            ?? throw new ArgumentNullException(nameof(repository));
    }

    public async Task<UpdateOrganizationNameResult?> ExecuteAsync(
        UpdateOrganizationNameCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        var organization = await _repository.GetByIdAsync(
            command.Id,
            cancellationToken);

        if (organization is null)
        {
            return null;
        }

        var previousName = organization.Name;

        organization.Rename(command.Name);

        var nameChanged = !string.Equals(
            previousName,
            organization.Name,
            StringComparison.OrdinalIgnoreCase);

        if (nameChanged &&
            await _repository.ExistsByNameAsync(
                organization.Name,
                cancellationToken))
        {
            throw new OrganizationNameAlreadyExistsException(
                organization.Name);
        }

        await _repository.UpdateAsync(
            organization,
            cancellationToken);

        return new UpdateOrganizationNameResult(
            organization.Id,
            organization.Name);
    }
}
