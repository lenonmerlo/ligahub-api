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
}