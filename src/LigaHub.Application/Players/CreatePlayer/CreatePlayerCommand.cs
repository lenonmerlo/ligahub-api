using LigaHub.Domain.Players;

namespace LigaHub.Application.Players.CreatePlayer;

public sealed record CreatePlayerCommand(
    Guid OrganizationId,
    Guid TeamId,
    string Name,
    DateOnly BirthDate,
    Sex Sex,
    int JerseyNumber);