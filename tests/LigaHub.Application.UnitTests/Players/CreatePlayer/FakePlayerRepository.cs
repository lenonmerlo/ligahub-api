using LigaHub.Application.Players;
using LigaHub.Domain.Players;

namespace LigaHub.Application.UnitTests.Players.CreatePlayer;

internal sealed class FakePlayerRepository : IPlayerRepository
{
    public bool JerseyNumberExists { get; set; }

    public Guid? RequestedTeamId { get; private set; }

    public int? RequestedJerseyNumber { get; private set; }

    public Player? AddedPlayer { get; private set; }

    public int ExistsCalls { get; private set; }

    public int AddCalls { get; private set; }

    public Task<bool> ExistsByJerseyNumberAsync(
        Guid teamId,
        int jerseyNumber,
        CancellationToken cancellationToken = default)
    {
        RequestedTeamId = teamId;
        RequestedJerseyNumber = jerseyNumber;
        ExistsCalls++;

        return Task.FromResult(JerseyNumberExists);
    }

    public Task AddAsync(
        Player player,
        CancellationToken cancellationToken = default)
    {
        AddedPlayer = player;
        AddCalls++;

        return Task.CompletedTask;
    }

    public Task<Player?> GetByIdAsync(
    Guid teamId,
    Guid id,
    CancellationToken cancellationToken = default)
    {
        return Task.FromResult<Player?>(null);
    }
}
