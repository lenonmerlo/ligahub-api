namespace LigaHub.Application.Teams.GetTeamById;

public sealed class GetTeamByIdUseCase
{
    private readonly ITeamRepository _repository;

    public GetTeamByIdUseCase(ITeamRepository repository)
    {
        _repository = repository
            ?? throw new ArgumentNullException(nameof(repository));
    }

    public async Task<GetTeamByIdResult?> ExecuteAsync(
        GetTeamByIdQuery query,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(query);

        var team = await _repository.GetByIdAsync(
            query.OrganizationId,
            query.Id,
            cancellationToken);

        if (team is null)
        {
            return null;
        }

        return new GetTeamByIdResult(
            team.Id,
            team.OrganizationId,
            team.Name);
    }
}
