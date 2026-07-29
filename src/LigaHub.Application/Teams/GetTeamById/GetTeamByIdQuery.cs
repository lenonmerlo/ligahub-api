namespace LigaHub.Application.Teams.GetTeamById;

public sealed record GetTeamByIdQuery(
    Guid OrganizationId,
    Guid Id);
