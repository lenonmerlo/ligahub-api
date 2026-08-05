using LigaHub.Domain.Players;

namespace LigaHub.Api.Contracts.Players;

public sealed record CreatePlayerRequest(
    string Name,
    DateOnly BirthDate,
    Sex Sex,
    int JerseyNumber);
