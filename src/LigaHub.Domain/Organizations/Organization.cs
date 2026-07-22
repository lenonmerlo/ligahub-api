namespace LigaHub.Domain.Organizations;

public sealed class Organization
{
    public const int MaxNameLength = 120;

    public Guid Id { get; }

    public string Name { get; private set; }

    private Organization(Guid id, string name)
    {
        Id = id;
        Name = name;
    }

    public static Organization Create(string name)
    {
        return new Organization(Guid.NewGuid(), NormalizeName(name));
    }

    private static string NormalizeName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException(
                "Organization name is required.",
                nameof(name));
        }

        var normalizedName = name.Trim();

        if (normalizedName.Length > MaxNameLength)
        {
            throw new ArgumentException(
                $"Organization name cannot exceed {MaxNameLength} characters.",
                nameof(name));
        }

        return normalizedName;
    }

}