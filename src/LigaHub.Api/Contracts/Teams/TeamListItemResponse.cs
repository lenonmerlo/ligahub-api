using LigaHub.Domain.Teams;

namespace LigaHub.Api.Contracts.Teams;

public sealed record TeamListItemResponse(
    Guid Id,
    string Name,
    Sport Sport);
