namespace LigaHub.Application.Players.CreatePlayer;

public sealed record CreatePlayerResult(
    Guid Id,
    Guid TeamId,
    string Name);
