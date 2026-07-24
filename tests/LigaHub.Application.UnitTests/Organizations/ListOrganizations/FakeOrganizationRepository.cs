using LigaHub.Application.Organizations;
using LigaHub.Domain.Organizations;

namespace LigaHub.Application.UnitTests.Organizations.ListOrganizations;

internal sealed class FakeOrganizationRepository
    : IOrganizationRepository
{
    public IReadOnlyList<Organization> Organizations { get; set; } = [];

    public int TotalCount { get; set; }

    public int? RequestedSkip { get; private set; }

    public int? RequestedTake { get; private set; }

    public Task<bool> ExistsByNameAsync(
        string name,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(false);
    }

    public Task AddAsync(
        Organization organization,
        CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public Task UpdateAsync(
    Organization organization,
    CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public Task<Organization?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult<Organization?>(null);
    }

    public Task<IReadOnlyList<Organization>> ListAsync(
        int skip,
        int take,
        CancellationToken cancellationToken = default)
    {
        RequestedSkip = skip;
        RequestedTake = take;

        return Task.FromResult(Organizations);
    }

    public Task<int> CountAsync(
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(TotalCount);
    }
}
