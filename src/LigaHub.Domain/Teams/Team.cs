namespace LigaHub.Domain.Teams;

public sealed class Team
{
    public const int MaxNameLength = 120;

    public Guid Id { get; }

    public Guid OrganizationId { get; }

    public Sport Sport { get; }

    public string Name { get; private set; }

    private Team(
        Guid id,
        Guid organizationId,
        string name,
        Sport sport)
    {
        Id = id;
        OrganizationId = organizationId;
        Name = name;
        Sport = sport;
    }

    public static Team Create(
        Guid organizationId,
        string name)
    {
        return Create(
            organizationId,
            name,
            Sport.Volleyball);
    }

    public static Team Create(
        Guid organizationId,
        string name,
        Sport sport)
    {
        if (organizationId == Guid.Empty)
        {
            throw new ArgumentException(
                "Organization id is required.",
                nameof(organizationId));
        }

        if (!Enum.IsDefined(sport))
        {
            throw new ArgumentOutOfRangeException(
                nameof(sport),
                "Sport is invalid.");
        }

        return new Team(
            Guid.NewGuid(),
            organizationId,
            NormalizeName(name),
            sport);
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
                "Team name is required.",
                nameof(name));
        }

        var normalizedName = name.Trim();

        if (normalizedName.Length > MaxNameLength)
        {
            throw new ArgumentException(
                $"Team name cannot exceed {MaxNameLength} characters.",
                nameof(name));
        }

        return normalizedName;
    }
}
