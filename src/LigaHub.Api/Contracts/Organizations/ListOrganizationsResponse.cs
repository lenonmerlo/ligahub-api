namespace LigaHub.Api.Contracts.Organizations;

public sealed record ListOrganizationsResponse(
    IReadOnlyList<OrganizationListItemResponse> Items,
    int Page,
    int PageSize,
    int TotalCount,
    int TotalPages);
