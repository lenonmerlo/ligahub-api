using LigaHub.Application.Organizations;
using LigaHub.Domain.Organizations;

namespace LigaHub.Application.UnitTests.Teams.ListTeams;

internal sealed class FakeOrganizationRepository
    : IOrganizationRepository
{
    public Organization? OrganizationToReturn { get; set; }

    public Guid? RequestedId { get; private set; }

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
        IReadOnlyList<Organization> organizations = [];

        return Task.FromResult(organizations);
    }

    public Task<int> CountAsync(
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(0);
    }
}
