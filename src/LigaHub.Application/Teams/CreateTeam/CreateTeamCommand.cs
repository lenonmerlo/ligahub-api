using LigaHub.Domain.Teams;

namespace LigaHub.Application.Teams.CreateTeam;

public sealed record CreateTeamCommand(
    Guid OrganizationId,
    string Name,
    Sport Sport = Sport.Volleyball);

