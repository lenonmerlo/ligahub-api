namespace LigaHub.Application.Organizations.ListOrganizations;

public sealed record ListOrganizationsResult(
    IReadOnlyList<OrganizationListItem> Items,
    int Page,
    int PageSize,
    int TotalCount,
    int TotalPages);
