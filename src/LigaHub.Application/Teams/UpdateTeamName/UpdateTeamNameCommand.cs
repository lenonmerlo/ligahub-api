namespace LigaHub.Application.Teams.UpdateTeamName;

public sealed record UpdateTeamNameCommand(
    Guid OrganizationId,
    Guid Id,
    string Name);