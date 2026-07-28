namespace LigaHub.Application.Organizations.DeleteOrganization;

public sealed class DeleteOrganizationUseCase
{
    private readonly IOrganizationRepository _repository;

    public DeleteOrganizationUseCase(
        IOrganizationRepository repository)
    {
        _repository = repository
            ?? throw new ArgumentNullException(nameof(repository));
    }

    public async Task<bool> ExecuteAsync(
        DeleteOrganizationCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        var organization = await _repository.GetByIdAsync(
            command.Id,
            cancellationToken);

        if (organization is null)
        {
            return false;
        }

        await _repository.DeleteAsync(
            organization,
            cancellationToken);

        return true;
    }
}
