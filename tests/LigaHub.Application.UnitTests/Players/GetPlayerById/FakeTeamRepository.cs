using LigaHub.Application.Teams;
using LigaHub.Domain.Teams;

namespace LigaHub.Application.UnitTests.Players.GetPlayerById;

internal sealed class FakeTeamRepository : ITeamRepository
{
    public Team? TeamToReturn { get; set; }

    public Guid? RequestedOrganizationId { get; private set; }

    public Guid? RequestedId { get; private set; }

    public int GetByIdCalls { get; private set; }

    public Task<bool> ExistsByNameAsync(
        Guid organizationId,
        string name,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(false);
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
        GetByIdCalls++;

        return Task.FromResult(TeamToReturn);
    }

    public Task<IReadOnlyList<Team>> ListAsync(
        Guid organizationId,
        int skip,
        int take,
        CancellationToken cancellationToken = default)
    {
        IReadOnlyList<Team> teams = Array.Empty<Team>();

        return Task.FromResult(teams);
    }

    public Task<int> CountAsync(
        Guid organizationId,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(0);
    }
}
