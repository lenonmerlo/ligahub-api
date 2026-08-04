using LigaHub.Application.Teams;
using LigaHub.Domain.Teams;

namespace LigaHub.Application.UnitTests.Teams.ListTeams;

internal sealed class FakeTeamRepository : ITeamRepository
{
    public IReadOnlyList<Team> Teams { get; set; } = [];

    public int TotalCount { get; set; }

    public Guid? RequestedListOrganizationId { get; private set; }

    public Guid? RequestedCountOrganizationId { get; private set; }

    public int? RequestedSkip { get; private set; }

    public int? RequestedTake { get; private set; }

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
        return Task.FromResult<Team?>(null);
    }

    public Task<IReadOnlyList<Team>> ListAsync(
        Guid organizationId,
        int skip,
        int take,
        CancellationToken cancellationToken = default)
    {
        RequestedListOrganizationId = organizationId;
        RequestedSkip = skip;
        RequestedTake = take;

        return Task.FromResult(Teams);
    }

    public Task<int> CountAsync(
        Guid organizationId,
        CancellationToken cancellationToken = default)
    {
        RequestedCountOrganizationId = organizationId;

        return Task.FromResult(TotalCount);
    }
}
