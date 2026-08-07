namespace LigaHub.Application.Players.GetPlayerById;

public sealed record GetPlayerByIdQuery(
    Guid OrganizationId,
    Guid TeamId,
    Guid Id);
