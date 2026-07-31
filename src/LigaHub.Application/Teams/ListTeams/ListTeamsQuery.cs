namespace LigaHub.Application.Teams.ListTeams;

public sealed record ListTeamsQuery(
    Guid OrganizationId,
    int Page = 1,
    int PageSize = 20);
