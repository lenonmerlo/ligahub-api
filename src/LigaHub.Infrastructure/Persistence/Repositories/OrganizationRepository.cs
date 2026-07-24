using LigaHub.Application.Organizations;
using LigaHub.Domain.Organizations;
using Microsoft.EntityFrameworkCore;

namespace LigaHub.Infrastructure.Persistence.Repositories;

public sealed class OrganizationRepository
    : IOrganizationRepository
{
    private readonly LigaHubDbContext _dbContext;

    public OrganizationRepository(LigaHubDbContext dbContext)
    {
        _dbContext = dbContext
            ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public Task<bool> ExistsByNameAsync(
        string name,
        CancellationToken cancellationToken = default)
    {
        return _dbContext.Organizations.AnyAsync(
            organization => organization.Name == name,
            cancellationToken);
    }

    public async Task AddAsync(
        Organization organization,
        CancellationToken cancellationToken = default)
    {
        await _dbContext.Organizations.AddAsync(
            organization,
            cancellationToken);

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public Task<Organization?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return _dbContext.Organizations.SingleOrDefaultAsync(
            organization => organization.Id == id,
            cancellationToken);
    }

    public async Task<IReadOnlyList<Organization>> ListAsync(
        int skip,
        int take,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Organizations
            .AsNoTracking()
            .OrderBy(organization => organization.Name)
            .Skip(skip)
            .Take(take)
            .ToListAsync(cancellationToken);
    }

    public Task<int> CountAsync(
        CancellationToken cancellationToken = default)
    {
        return _dbContext.Organizations.CountAsync(cancellationToken);
    }
}
