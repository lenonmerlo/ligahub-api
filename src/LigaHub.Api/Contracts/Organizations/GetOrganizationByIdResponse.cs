namespace LigaHub.Api.Contracts.Organizations;

public sealed record GetOrganizationByIdResponse(
    Guid Id,
    string Name);
