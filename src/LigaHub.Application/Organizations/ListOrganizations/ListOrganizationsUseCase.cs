namespace LigaHub.Application.Organizations.ListOrganizations;

public sealed class ListOrganizationsUseCase
{
    public const int MaxPageSize = 100;

    private readonly IOrganizationRepository _repository;

    public ListOrganizationsUseCase(
        IOrganizationRepository repository)
    {
        _repository = repository
            ?? throw new ArgumentNullException(nameof(repository));
    }

    public async Task<ListOrganizationsResult> ExecuteAsync(
        ListOrganizationsQuery query,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(query);

        if (query.Page < 1)
        {
            throw new ArgumentNullException(
                nameof(query.Page),
                "Page must be greater than zero.");
        }

        if (query.PageSize < 1 || query.PageSize > MaxPageSize)
        {
            throw new ArgumentOutOfRangeException(
                nameof(query.PageSize),
                $"Page size must be between 1 and {MaxPageSize}.");
        }

        if (query.Page - 1 > int.MaxValue / query.PageSize)
        {
            throw new ArgumentOutOfRangeException(
                nameof(query.Page),
                "Page is too large.");
        }

        var skip = (query.Page - 1) * query.PageSize;

        var organizations = await _repository.ListAsync(
            skip,
            query.PageSize,
            cancellationToken);

        var totalCount = await _repository.CountAsync(
            cancellationToken);

        var items = organizations
            .Select(organization => new OrganizationListItem(
                organization.Id,
                organization.Name))
            .ToArray();

        var totalPages = (int)Math.Ceiling(
            totalCount / (double)query.PageSize);

        return new ListOrganizationsResult(
            items,
            query.Page,
            query.PageSize,
            totalCount,
            totalPages);
    }
}
