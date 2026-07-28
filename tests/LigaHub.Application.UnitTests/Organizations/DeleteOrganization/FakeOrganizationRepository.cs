using LigaHub.Application.Organizations;
using LigaHub.Domain.Organizations;

namespace LigaHub.Application.UnitTests.Organizations.DeleteOrganization;

internal sealed class FakeOrganizationRepository
    : IOrganizationRepository
{
    public Organization? OrganizationToReturn { get; set; }

    public Organization? DeletedOrganization { get; private set; }

    public Guid? RequestedId { get; private set; }

    public int DeleteCalls { get; private set; }

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

    public Task DeleteAsync(
        Organization organization,
        CancellationToken cancellationToken = default)
    {
        DeletedOrganization = organization;
        DeleteCalls++;

        return Task.CompletedTask;
    }

    public Task<Organization?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        RequestedId = id;

        return Task.FromResult(OrganizationToReturn);
    }

    public Task<IReadOnlyList<Organization>> ListAsync(
        int skip,
        int take,
        CancellationToken cancellationToken = default)
    {
        IReadOnlyList<Organization> organizations =
            Array.Empty<Organization>();

        return Task.FromResult(organizations);
    }

    public Task<int> CountAsync(
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(0);
    }
}
