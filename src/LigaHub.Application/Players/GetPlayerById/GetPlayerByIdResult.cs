using LigaHub.Domain.Players;

namespace LigaHub.Application.Players.GetPlayerById;

public sealed record GetPlayerByIdResult(
    Guid Id,
    Guid TeamId,
    string Name,
    DateOnly BirthDate,
    Sex Sex,
    int JerseyNumber);
