using LigaHub.Application.Organizations;

namespace LigaHub.Application.Teams.ListTeams;

public sealed class ListTeamsUseCase
{
    public const int MaxPageSize = 100;

    private readonly IOrganizationRepository _organizationRepository;
    private readonly ITeamRepository _teamRepository;

    public ListTeamsUseCase(
        IOrganizationRepository organizationRepository,
        ITeamRepository teamRepository)
    {
        _organizationRepository = organizationRepository
            ?? throw new ArgumentNullException(
                nameof(organizationRepository));

        _teamRepository = teamRepository
            ?? throw new ArgumentNullException(
                nameof(teamRepository));
    }

    public async Task<ListTeamsResult?> ExecuteAsync(
        ListTeamsQuery query,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(query);

        if (query.OrganizationId == Guid.Empty)
        {
            throw new ArgumentException(
                "Organization id is required.",
                nameof(query.OrganizationId));
        }

        if (query.Page < 1)
        {
            throw new ArgumentOutOfRangeException(
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

        var organization = await _organizationRepository.GetByIdAsync(
            query.OrganizationId,
            cancellationToken);

        if (organization is null)
        {
            return null;
        }

        var skip = (query.Page - 1) * query.PageSize;

        var teams = await _teamRepository.ListAsync(
            query.OrganizationId,
            skip,
            query.PageSize,
            cancellationToken);

        var totalCount = await _teamRepository.CountAsync(
            query.OrganizationId,
            cancellationToken);

        var items = teams
            .Select(team => new TeamListItem(
                team.Id,
                team.Name))
            .ToArray();

        var totalPages = (int)Math.Ceiling(
            totalCount / (double)query.PageSize);

        return new ListTeamsResult(
            items,
            query.Page,
            query.PageSize,
            totalCount,
            totalPages);
    }
}
