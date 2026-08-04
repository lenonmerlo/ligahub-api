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

    public async Task AddAsync(
        Team team,
        CancellationToken cancellationToken = default)
    {
        await _dbContext.Teams.AddAsync(
            team,
            cancellationToken);

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(
        Team team,
        CancellationToken cancellationToken = default)
    {
        _dbContext.Teams.Update(team);

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public Task<Team?> GetByIdAsync(
        Guid organizationId,
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return _dbContext.Teams.SingleOrDefaultAsync(
            team =>
                team.OrganizationId == organizationId &&
                team.Id == id,
            cancellationToken);
    }

    public async Task<IReadOnlyList<Team>> ListAsync(
        Guid ornigazationId,
        int skip,
        int take,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Teams
            .AsNoTracking()
            .Where(team => team.OrganizationId == ornigazationId)
            .OrderBy(team => team.Name)
            .ThenBy(team => team.Id)
            .Skip(skip)
            .Take(take)
            .ToListAsync(cancellationToken);
    }

    public Task<int> CountAsync(
        Guid organizationId,
        CancellationToken cancellationToken = default)
    {
        return _dbContext.Teams.CountAsync(
            team => team.OrganizationId == organizationId,
            cancellationToken);
    }
}
