using LigaHub.Application.Organizations;
using LigaHub.Domain.Teams;

namespace LigaHub.Application.Teams.CreateTeam;

public sealed class CreateTeamUseCase
{
    private readonly IOrganizationRepository _organizationRepository;
    private readonly ITeamRepository _teamRepository;

    public CreateTeamUseCase(
        IOrganizationRepository organizationRepository,
        ITeamRepository teamRepository)
    {
        _organizationRepository = organizationRepository
            ?? throw new ArgumentNullException(
                nameof(organizationRepository));

        _teamRepository = teamRepository
            ?? throw new ArgumentNullException(
                nameof(teamRepository));
    }

    public async Task<CreateTeamResult?> ExecuteAsync(
        CreateTeamCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        var team = Team.Create(
            command.OrganizationId,
            command.Name);

        var organization = await _organizationRepository.GetByIdAsync(
            command.OrganizationId,
            cancellationToken);

        if (organization is null)
        {
            return null;
        }

        var nameAlreadyExists = await _teamRepository.ExistsByNameAsync(
            team.OrganizationId,
            team.Name,
            cancellationToken);

        if (nameAlreadyExists)
        {
            throw new TeamNameAlreadyExistsException(team.Name);
        }

        await _teamRepository.AddAsync(
            team,
            cancellationToken);

        return new CreateTeamResult(
            team.Id,
            team.OrganizationId,
            team.Name);
    }
}
