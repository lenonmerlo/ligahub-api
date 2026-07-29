namespace LigaHub.Api.Contracts.Teams;

public sealed record CreateTeamResponse(
    Guid Id,
    Guid OrganizationId,
    string Name);
