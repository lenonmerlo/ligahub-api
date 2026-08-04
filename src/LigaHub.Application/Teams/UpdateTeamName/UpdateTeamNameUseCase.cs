namespace LigaHub.Application.Teams.UpdateTeamName;

public sealed class UpdateTeamNameUseCase
{
    private readonly ITeamRepository _repository;

    public UpdateTeamNameUseCase(ITeamRepository repository)
    {
        _repository = repository
            ?? throw new ArgumentNullException(nameof(repository));
    }

    public async Task<UpdateTeamNameResult?> ExecuteAsync(
        UpdateTeamNameCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        var team = await _repository.GetByIdAsync(
            command.OrganizationId,
            command.Id,
            cancellationToken);

        if (team is null)
        {
            return null;
        }

        var previousName = team.Name;

        team.Rename(command.Name);

        var nameChanged = !string.Equals(
            previousName,
            team.Name,
            StringComparison.OrdinalIgnoreCase);

        if (nameChanged &&
            await _repository.ExistsByNameAsync(
                team.OrganizationId,
                team.Name,
                cancellationToken))
        {
            throw new TeamNameAlreadyExistsException(team.Name);
        }

        await _repository.UpdateAsync(
            team,
            cancellationToken);

        return new UpdateTeamNameResult(
            team.Id,
            team.OrganizationId,
            team.Name);
    }
}
