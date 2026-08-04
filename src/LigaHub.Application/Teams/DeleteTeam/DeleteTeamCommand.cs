namespace LigaHub.Application.Teams.DeleteTeam;

public sealed record DeleteTeamCommand(
    Guid OrganizationId,
    Guid Id);
