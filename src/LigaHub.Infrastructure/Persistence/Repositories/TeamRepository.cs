using LigaHub.Application.Teams;
using LigaHub.Domain.Teams;
using Microsoft.EntityFrameworkCore;

namespace LigaHub.Infrastructure.Persistence.Repositories;

public sealed class TeamRepository : ITeamRepository
{
    private readonly LigaHubDbContext _dbContext;

    public TeamRepository(LigaHubDbContext dbContext)
    {
        _dbContext = dbContext
            ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public Task<bool> ExistsByNameAsync(
        Guid organizationId,
        string name,
        CancellationToken cancellationToken = default)
    {
        return _dbContext.Teams.AnyAsync(
            team =>
                team.OrganizationId == organizationId &&
                team.Name == name,
            cancellationToken);
    }

    public async Task TaskAsync(
        Team team,
        CancellationToken cancellationToken = default)
    {
        await _dbContext.Teams.AddAsync(
            team,
            cancellationToken);

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    Task ITeamRepository.AddAsync(Team team, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    Task<bool> ITeamRepository.ExistsByNameAsync(Guid organizationId, string name, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
