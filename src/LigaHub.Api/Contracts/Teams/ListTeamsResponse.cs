namespace LigaHub.Api.Contracts.Teams;

public sealed record ListTeamsResponse(
    IReadOnlyList<TeamListItemResponse> Items,
    int Page,
    int PageSize,
    int TotalCount,
    int TotalPages);
