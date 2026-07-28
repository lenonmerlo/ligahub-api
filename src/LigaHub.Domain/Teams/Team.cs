namespace LigaHub.Domain.Teams;

public sealed class Team
{
    public const int MaxNameLength = 120;

    public Guid Id { get; }

    public Guid OrganizationId { get; }

    public string Name { get; private set; }

    private Team(
        Guid id,
        Guid organizationId,
        string name)
    {
        Id = id;
        OrganizationId = organizationId;
        Name = name;
    }

    public static Team Create(
        Guid organizationId,
        string name)
    {
        if (organizationId == Guid.Empty)
        {
            throw new ArgumentException(
                "Organization id is required.",
                nameof(organizationId));
        }

        return new Team(
            Guid.NewGuid(),
            organizationId,
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
