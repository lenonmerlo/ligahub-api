namespace LigaHub.Application.Teams.CreateTeam;

public sealed record CreateTeamCommand(
    Guid OrganizationId,
    string Name);

