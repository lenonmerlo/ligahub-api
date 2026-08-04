using LigaHub.Application.Players;
using LigaHub.Domain.Players;
using Microsoft.EntityFrameworkCore;

namespace LigaHub.Infrastructure.Persistence.Repositories;

public sealed class PlayerRepository : IPlayerRepository
{
    private readonly LigaHubDbContext _dbContext;

    public PlayerRepository(LigaHubDbContext dbContext)
    {
        _dbContext = dbContext
            ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public Task<bool> ExistsByNameAsync(
        Guid teamId,
        string name,
        CancellationToken cancellationToken = default)
    {
        return _dbContext.Players.AnyAsync(
            player =>
                player.TeamId == teamId &&
                player.Name == name,
            cancellationToken);
    }

    public async Task AddAsync(
        Player player,
        CancellationToken cancellationToken = default)
    {
        await _dbContext.Players.AddAsync(
            player,
            cancellationToken);

        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
