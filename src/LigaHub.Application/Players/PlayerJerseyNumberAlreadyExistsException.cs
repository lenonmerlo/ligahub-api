namespace LigaHub.Application.Players;

public sealed class PlayerJerseyNumberAlreadyExistsException : Exception
{
    public PlayerJerseyNumberAlreadyExistsException(
        int jerseyNumber)
        : base(
            $"Jersey number '{jerseyNumber}' already exists in this team.")
    { }
}
