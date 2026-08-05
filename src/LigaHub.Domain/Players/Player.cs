namespace LigaHub.Domain.Players;

public sealed class Player
{
    public const int MaxNameLength = 120;
    public const int MinJerseyNumber = 1;
    public const int MaxJerseyNumber = 99;

    public Guid Id { get; }

    public Guid TeamId { get; }

    public string Name { get; private set; }

    public DateOnly BirthDate { get; }

    public Sex Sex { get; }

    public int JerseyNumber { get; }

    private Player(
        Guid id,
        Guid teamId,
        string name,
        DateOnly birthDate,
        Sex sex,
        int jerseyNumber)
    {
        Id = id;
        TeamId = teamId;
        Name = name;
        BirthDate = birthDate;
        Sex = sex;
        JerseyNumber = jerseyNumber;
    }

    public static Player Create(
        Guid teamId,
        string name,
        DateOnly birthDate,
        Sex sex,
        int jerseyNumber)
    {
        if (teamId == Guid.Empty)
        {
            throw new ArgumentException(
                "Team id is required.",
                nameof(teamId));
        }

        if (birthDate == default)
        {
            throw new ArgumentException(
                "Player birth date is required.",
                nameof(birthDate));
        }

        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        if (birthDate > today)
        {
            throw new ArgumentOutOfRangeException(
                nameof(birthDate),
                "Player birth date cannot be in the future.");
        }

        if (!Enum.IsDefined(sex))
        {
            throw new ArgumentOutOfRangeException(
                nameof(sex),
                "Player sex is invalid.");
        }

        if (jerseyNumber < MinJerseyNumber || jerseyNumber > MaxJerseyNumber)
        {
            throw new ArgumentOutOfRangeException(
                nameof(jerseyNumber),
                $"Jersey number must be between {MinJerseyNumber} and {MaxJerseyNumber}.");
        }

        return new Player(
            Guid.NewGuid(),
            teamId,
            NormalizeName(name),
            birthDate,
            sex,
            jerseyNumber);
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
