using LigaHub.Application.Teams;
using LigaHub.Domain.Teams;

namespace LigaHub.Application.UnitTests.Teams.UpdateTeamName;

internal sealed class FakeTeamRepository : ITeamRepository
{
    public Team? TeamToReturn { get; set; }

    public bool NameExists { get; set; }

    public Team? UpdatedTeam { get; private set; }

    public Guid? RequestedOrganizationId { get; private set; }

    public Guid? RequestedId { get; private set; }

    public Guid? NameCheckOrganizationId { get; private set; }

    public string? RequestedName { get; private set; }

    public int UpdateCalls { get; private set; }

    public int ExistsByNameCalls { get; private set; }

    public Task<bool> ExistsByNameAsync(
        Guid organizationId,
        string name,
        CancellationToken cancellationToken = default)
    {
        NameCheckOrganizationId = organizationId;
        RequestedName = name;
        ExistsByNameCalls++;

        return Task.FromResult(NameExists);
    }

    public Task AddAsync(
        Team team,
        CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public Task UpdateAsync(
        Team team,
        CancellationToken cancellationToken = default)
    {
        UpdatedTeam = team;
        UpdateCalls++;

        return Task.CompletedTask;
    }

    public Task DeleteAsync(
    Team team,
    CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public Task<Team?> GetByIdAsync(
        Guid organizationId,
        Guid id,
        CancellationToken cancellationToken = default)
    {
        RequestedOrganizationId = organizationId;
        RequestedId = id;

        return Task.FromResult(TeamToReturn);
    }

    public Task<IReadOnlyList<Team>> ListAsync(
        Guid organizationId,
        int skip,
        int take,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult<IReadOnlyList<Team>>([]);
    }

    public Task<int> CountAsync(
        Guid organizationId,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(0);
    }
}
