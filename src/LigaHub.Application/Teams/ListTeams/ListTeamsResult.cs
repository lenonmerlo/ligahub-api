namespace LigaHub.Application.Teams.ListTeams;

public sealed record ListTeamsResult(
    IReadOnlyList<TeamListItem> Items,
    int Page,
    int PageSize,
    int TotalCount,
    int TotalPages);
