using LigaHub.Domain.Players;

namespace LigaHub.Application.Players;

public interface IPlayerRepository
{
    Task<bool> ExistsByJerseyNumberAsync(
        Guid teamId,
        int jerseyNumber,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        Player player,
        CancellationToken cancellationToken = default);

    Task<Player?> GetByIdAsync(
        Guid teamId,
        Guid id,
        CancellationToken cancellationToken = default);
}
