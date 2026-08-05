using LigaHub.Domain.Teams;

namespace LigaHub.Api.Contracts.Teams;

public sealed record CreateTeamRequest(
    string Name,
    Sport Sport = Sport.Volleyball);
