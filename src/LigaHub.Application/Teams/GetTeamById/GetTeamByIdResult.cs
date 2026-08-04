using LigaHub.Domain.Teams;

namespace LigaHub.Application.Teams.GetTeamById;

public sealed record GetTeamByIdResult(
    Guid Id,
    Guid OrganizationId,
    string Name,
    Sport Sport);
