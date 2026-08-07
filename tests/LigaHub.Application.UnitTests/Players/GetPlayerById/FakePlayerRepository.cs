using LigaHub.Application.Players;
using LigaHub.Domain.Players;

namespace LigaHub.Application.UnitTests.Players.GetPlayerById;

internal sealed class FakePlayerRepository : IPlayerRepository
{
    public Player? PlayerToReturn { get; set; }

    public Guid? RequestedTeamId { get; private set; }

    public Guid? RequestedId { get; private set; }

    public int GetByIdCalls { get; private set; }

    public Task<bool> ExistsByJerseyNumberAsync(
        Guid teamId,
        int jerseyNumber,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(false);
    }

    public Task AddAsync(
        Player player,
        CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public Task<Player?> GetByIdAsync(
        Guid teamId,
        Guid id,
        CancellationToken cancellationToken = default)
    {
        RequestedTeamId = teamId;
        RequestedId = id;
        GetByIdCalls++;

        return Task.FromResult(PlayerToReturn);
    }
}
