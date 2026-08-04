using LigaHub.Domain.Teams;

namespace LigaHub.Application.Teams;

public interface ITeamRepository
{
    Task<bool> ExistsByNameAsync(
        Guid organizationId,
        string name,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        Team team,
        CancellationToken cancellationToken = default);

    Task UpdateAsync(
        Team team,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(
        Team team,
        CancellationToken cancellationToken = default);

    Task<Team?> GetByIdAsync(
        Guid organizationId,
        Guid id,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Team>> ListAsync(
        Guid organizationId,
        int skip,
        int take,
        CancellationToken cancellationToken = default);

    Task<int> CountAsync(
        Guid organizationId,
        CancellationToken cancellationToken = default);
}
