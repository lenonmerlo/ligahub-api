using LigaHub.Application.Organizations;
using LigaHub.Domain.Organizations;

namespace LigaHub.Application.UnitTests.Organizations.GetOrganizationById;

internal sealed class FakeOrganizationRepository : IOrganizationRepository
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

    public Task<Organization?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        RequestedId = id;

        return Task.FromResult(OrganizationToReturn);
    }
}
