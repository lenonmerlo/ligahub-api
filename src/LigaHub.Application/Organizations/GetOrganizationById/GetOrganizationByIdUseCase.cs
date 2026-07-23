namespace LigaHub.Application.Organizations.GetOrganizationById;

public sealed class GetOrganizationByIdUseCase
{
    private readonly IOrganizationRepository _repository;

    public GetOrganizationByIdUseCase(
        IOrganizationRepository repository)
    {
        _repository = repository
            ?? throw new ArgumentNullException(nameof(repository));
    }

    public async Task<GetOrganizationByIdResult?> ExecuteAsync(
        GetOrganizationByIdQuery query,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(query);

        var organization = await _repository.GetByIdAsync(
            query.Id,
            cancellationToken);

        if (organization is null)
        {
            return null;
        }

        return new GetOrganizationByIdResult(
            organization.Id,
            organization.Name);
    }
}
