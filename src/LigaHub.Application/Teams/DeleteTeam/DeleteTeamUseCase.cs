namespace LigaHub.Application.Teams.DeleteTeam;

public sealed class DeleteTeamUseCase
{
    private readonly ITeamRepository _repository;

    public DeleteTeamUseCase(
        ITeamRepository repository)
    {
        _repository = repository
            ?? throw new ArgumentNullException(nameof(repository));
    }

    public async Task<bool> ExecuteAsync(
        DeleteTeamCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        var team = await _repository.GetByIdAsync(
            command.OrganizationId,
            command.Id,
            cancellationToken);

        if (team is null)
        {
            return false;
        }

        await _repository.DeleteAsync(
            team,
            cancellationToken);

        return true;
    }
}
