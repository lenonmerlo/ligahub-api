using LigaHub.Application.Organizations;
using LigaHub.Domain.Organizations;

namespace LigaHub.Application.UnitTests.Organizations.UpdateOrganizationName;

internal sealed class FakeOrganizationRepository
    : IOrganizationRepository
{
    public Organization? OrganizationToReturn { get; set; }

    public bool NameExists { get; set; }

    public Organization? UpdatedOrganization { get; private set; }

    public int UpdateCalls { get; private set; }

    public int ExistsByNameCalls { get; private set; }

    public Task<bool> ExistsByNameAsync(
        string name,
        CancellationToken cancellationToken = default)
    {
        ExistsByNameCalls++;

        return Task.FromResult(NameExists);
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
        UpdatedOrganization = organization;
        UpdateCalls++;

        return Task.CompletedTask;
    }

    public Task DeleteAsync(
    Organization organization,
    CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public Task<Organization?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(OrganizationToReturn);
    }

    public Task<IReadOnlyList<Organization>> ListAsync(
        int skip,
        int take,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult<IReadOnlyList<Organization>>([]);
    }

    public Task<int> CountAsync(
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(0);
    }
}
