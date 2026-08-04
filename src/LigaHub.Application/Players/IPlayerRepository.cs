using LigaHub.Domain.Players;

namespace LigaHub.Application.Players;

public interface IPlayerRepository
{
    Task<bool> ExistsByNameAsync(
        Guid teamId,
        string name,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        Player player,
        CancellationToken cancellationToken = default);
}
