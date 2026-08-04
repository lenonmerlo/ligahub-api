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
            command.Name);

        var team = await _teamRepository.GetByIdAsync(
            command.OrganizationId,
            command.TeamId,
            cancellationToken);

        if (team is null)
        {
            return null;
        }

        var nameAlreadyExists =
            await _playerRepository.ExistsByNameAsync(
                player.TeamId,
                player.Name,
                cancellationToken);

        if (nameAlreadyExists)
        {
            throw new PlayerNameAlreadyExistsException(
                player.Name);
        }

        await _playerRepository.AddAsync(
            player,
            cancellationToken);

        return new CreatePlayerResult(
            player.Id,
            player.TeamId,
            player.Name);
    }
}