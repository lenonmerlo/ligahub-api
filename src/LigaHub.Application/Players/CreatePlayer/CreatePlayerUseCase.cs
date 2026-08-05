using LigaHub.Application.Teams;
using LigaHub.Domain.Players;

namespace LigaHub.Application.Players.CreatePlayer;

public sealed class CreatePlayerUseCase
{
    private readonly ITeamRepository _teamRepository;
    private readonly IPlayerRepository _playerRepository;

    public CreatePlayerUseCase(
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

    public async Task<CreatePlayerResult?> ExecuteAsync(
        CreatePlayerCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        var player = Player.Create(
            command.TeamId,
            command.Name,
            command.BirthDate,
            command.Sex,
            command.JerseyNumber);

        var team = await _teamRepository.GetByIdAsync(
            command.OrganizationId,
            command.TeamId,
            cancellationToken);

        if (team is null)
        {
            return null;
        }

        var jerseyNumberAlreadyExists =
            await _playerRepository.ExistsByJerseyNumberAsync(
                player.TeamId,
                player.JerseyNumber,
                cancellationToken);

        if (jerseyNumberAlreadyExists)
        {
            throw new PlayerJerseyNumberAlreadyExistsException(
                player.JerseyNumber);
        }

        await _playerRepository.AddAsync(
            player,
            cancellationToken);

        return new CreatePlayerResult(
            player.Id,
            player.TeamId,
            player.Name,
            player.BirthDate,
            player.Sex,
            player.JerseyNumber);
    }
}