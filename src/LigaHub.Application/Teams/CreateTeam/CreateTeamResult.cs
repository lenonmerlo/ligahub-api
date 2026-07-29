namespace LigaHub.Application.Teams.CreateTeam;

public sealed record CreateTeamResult(
    Guid Id,
    Guid OrganizationId,
    string Name);
