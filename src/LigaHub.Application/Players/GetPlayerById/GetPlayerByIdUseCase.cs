using LigaHub.Application.Teams;

namespace LigaHub.Application.Players.GetPlayerById;

public sealed class GetPlayerByIdUseCase
{
    private readonly ITeamRepository _teamRepository;
    private readonly IPlayerRepository _playerRepository;

    public GetPlayerByIdUseCase(
        ITeamRepository teamRepository,
        IPlayerRepository playerRepository)
    {
        _teamRepository = teamRepository
            ?? throw new ArgumentNullException(
                nameof(teamRepository));

        _playerRepository = playerRepository
            ?? throw new ArgumentNullException(
                nameof(playerRepository));
    }

    public async Task<GetPlayerByIdResult?> ExecuteAsync(
        GetPlayerByIdQuery query,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(query);

        var team = await _teamRepository.GetByIdAsync(
            query.OrganizationId,
            query.TeamId,
            cancellationToken);

        if (team is null)
        {
            return null;
        }

        var player = await _playerRepository.GetByIdAsync(
            query.TeamId,
            query.Id,
            cancellationToken);

        if (player is null)
        {
            return null;
        }

        return new GetPlayerByIdResult(
            player.Id,
            player.TeamId,
            player.Name,
            player.BirthDate,
            player.Sex,
            player.JerseyNumber);
    }
}
