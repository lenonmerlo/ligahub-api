namespace LigaHub.Api.Contracts.Teams;

public sealed record UpdateTeamNameResponse(
    Guid Id,
    Guid OrganizationId,
    string Name);