namespace LigaHub.Application.Organizations.ListOrganizations;

public sealed record ListOrganizationsQuery(
    int Page = 1,
    int PageSize = 20);
