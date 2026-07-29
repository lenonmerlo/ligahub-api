namespace LigaHub.Application.Teams;

public sealed class TeamNameAlreadyExistsException : Exception
{
    public TeamNameAlreadyExistsException(string name)
        : base($"A team named '{name}' already exists in this organization.") { }
}
