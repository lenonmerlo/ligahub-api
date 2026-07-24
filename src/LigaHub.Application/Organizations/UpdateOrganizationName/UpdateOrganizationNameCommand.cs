namespace LigaHub.Application.Organizations.UpdateOrganizationName;

public sealed record UpdateOrganizationNameCommand(
    Guid Id,
    string Name);
