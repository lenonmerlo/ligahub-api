namespace LigaHub.Api.Contracts.Organizations;

public sealed record CreateOrganizationResponse(
    Guid Id,
    string Name);
