using LigaHub.Domain.Players;

namespace LigaHub.Application.Players.CreatePlayer;

public sealed record CreatePlayerResult(
    Guid Id,
    Guid TeamId,
    string Name,
    DateOnly BirthDate,
    Sex Sex,
    int JerseyNumber);
