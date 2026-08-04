using LigaHub.Application.Players;
using LigaHub.Domain.Players;

namespace LigaHub.Application.UnitTests.Players.CreatePlayer;

internal sealed class FakePlayerRepository : IPlayerRepository
{
    public bool NameExists { get; set; }

    public Guid? RequestedTeamId { get; private set; }

    public string? RequestedName { get; private set; }

    public Player? AddedPlayer { get; private set; }

    public int ExistsCalls { get; private set; }

    public int AddCalls { get; private set; }

    public Task<bool> ExistsByNameAsync(
        Guid teamId,
        string name,
        CancellationToken cancellationToken = default)
    {
        RequestedTeamId = teamId;
        RequestedName = name;
        ExistsCalls++;

        return Task.FromResult(NameExists);
    }

    public Task AddAsync(
        Player player,
        CancellationToken cancellationToken = default)
    {
        AddedPlayer = player;
        AddCalls++;

        return Task.CompletedTask;
    }
}
