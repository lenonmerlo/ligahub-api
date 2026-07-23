using LigaHub.Application.Organizations;
using LigaHub.Domain.Organizations;

namespace LigaHub.Application.UnitTests.Organizations.CreateOrganization;

internal sealed class FakeOrganizationRepository
    : IOrganizationRepository
{
    public bool NameExists { get; set; }

    public Organization? AddedOrganization { get; private set; }

    public int AddCalls { get; private set; }

    public Task<bool> ExistsByNameAsync(
        string name,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(NameExists);
    }

    public Task AddAsync(
        Organization organization,
        CancellationToken cancellationToken = default)
    {
        AddedOrganization = organization;
        AddCalls++;

        return Task.CompletedTask;
    }

    public Task<Organization?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult<Organization?>(null);
    }
}
