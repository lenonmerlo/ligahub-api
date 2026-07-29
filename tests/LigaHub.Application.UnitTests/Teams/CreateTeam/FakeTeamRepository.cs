using LigaHub.Application.Teams;
using LigaHub.Domain.Teams;

namespace LigaHub.Application.UnitTests.Teams.CreateTeam;

internal sealed class FakeTeamRepository : ITeamRepository
{
    public bool NameExists { get; set; }

    public Guid? RequestedOrganizationId { get; private set; }

    public string? RequestedName { get; private set; }

    public Team? AddedTeam { get; private set; }

    public int ExistsCalls { get; private set; }

    public int AddCalls { get; private set; }

    public Task<bool> ExistsByNameAsync(
        Guid organizationId,
        string name,
        CancellationToken cancellationToken = default)
    {
        RequestedOrganizationId = organizationId;
        RequestedName = name;
        ExistsCalls++;

        return Task.FromResult(NameExists);
    }

    public Task AddAsync(
        Team team,
        CancellationToken cancellationToken = default)
    {
        AddedTeam = team;
        AddCalls++;

        return Task.CompletedTask;
    }
}
