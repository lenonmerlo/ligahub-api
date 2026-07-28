namespace LigaHub.Api.Contracts.Organizations;

public sealed record UpdateOrganizationNameResponse(
    Guid Id,
    string Name);
