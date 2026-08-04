namespace LigaHub.Application.Players;

public sealed class PlayerNameAlreadyExistsException : Exception
{
    public PlayerNameAlreadyExistsException(string name)
        : base(
            $"A player named '{name}' already exists in this team.")
    {
    }
}