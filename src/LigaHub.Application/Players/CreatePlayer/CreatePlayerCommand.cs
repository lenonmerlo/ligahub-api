namespace LigaHub.Application.Players.CreatePlayer;

public sealed record CreatePlayerCommand(
    Guid OrganizationId,
    Guid TeamId,
    string Name);
