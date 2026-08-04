namespace LigaHub.Domain.Players;

public sealed class Player
{
    public const int MaxNameLength = 120;

    public Guid Id { get; }

    public Guid TeamId { get; }

    public string Name { get; private set; }

    private Player(
        Guid id,
        Guid teamId,
        string name)
    {
        Id = id;
        TeamId = teamId;
        Name = name;
    }

    public static Player Create(
        Guid teamId,
        string name)
    {
        if (teamId == Guid.Empty)
        {
            throw new ArgumentException(
                "Team id is required.",
                nameof(teamId));
        }

        return new Player(
            Guid.NewGuid(),
            teamId,
            NormalizeName(name));
    }

    public void Rename(string name)
    {
        Name = NormalizeName(name);
    }

    private static string NormalizeName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException(
                "Player name is required.",
                nameof(name));
        }

        var normalizedName = name.Trim();

        if (normalizedName.Length > MaxNameLength)
        {
            throw new ArgumentException(
                $"Player name cannot exceed {MaxNameLength} characters.",
                nameof(name));
        }

        return normalizedName;
    }
}
