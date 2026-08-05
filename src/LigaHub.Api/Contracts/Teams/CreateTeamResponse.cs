using LigaHub.Domain.Teams;

namespace LigaHub.Api.Contracts.Teams;

public sealed record CreateTeamResponse(
    Guid Id,
    Guid OrganizationId,
    string Name,
    Sport Sport);
