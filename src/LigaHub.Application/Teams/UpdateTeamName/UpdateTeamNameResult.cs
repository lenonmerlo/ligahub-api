using LigaHub.Domain.Teams;

namespace LigaHub.Application.Teams.UpdateTeamName;

public sealed record UpdateTeamNameResult(
    Guid Id,
    Guid OrganizationId,
    string Name,
    Sport Sport);