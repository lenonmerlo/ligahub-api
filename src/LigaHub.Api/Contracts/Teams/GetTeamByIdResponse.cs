using LigaHub.Domain.Teams;

namespace LigaHub.Api.Contracts.Teams;

public sealed record GetTeamByIdResponse(
    Guid Id,
    Guid OrganizationId,
    string Name,
    Sport Sport);