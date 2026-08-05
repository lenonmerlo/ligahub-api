using LigaHub.Domain.Players;

namespace LigaHub.Api.Contracts.Players;

public sealed record CreatePlayerResponse(
    Guid Id,
    Guid TeamId,
    string Name,
    DateOnly BirthDate,
    Sex Sex,
    int JerseyNumber);
