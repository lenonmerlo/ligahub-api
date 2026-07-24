using System.Net;
using System.Net.Http.Json;

using LigaHub.Api.Contracts.Organizations;
using LigaHub.Api.FunctionalTests.Database;

using Microsoft.AspNetCore.Mvc;

namespace LigaHub.Api.FunctionalTests.Organizations;

public sealed class ListOrganizationsEndpointTests
    : IClassFixture<LigaHubApiFactory>
{
    private readonly HttpClient _client;

    public ListOrganizationsEndpointTests(
        LigaHubApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task List_ShouldReturnPaginatedOrganizations()
    {
        var prefix = $"Pagination {Guid.NewGuid():N}";

        var names = new[]
        {
            $"{prefix} C",
            $"{prefix} A",
            $"{prefix} B"
        };

        foreach (var name in names)
        {
            var request = new CreateOrganizationRequest(name);

            var createResponse = await _client.PostAsJsonAsync(
                "/api/organizations",
                request);

            Assert.Equal(
                HttpStatusCode.Created,
                createResponse.StatusCode);
        }

        var response = await _client.GetAsync(
            "/api/organizations?page=2&pageSize=1");

        var content = await response.Content
            .ReadFromJsonAsync<ListOrganizationsResponse>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(content);
        Assert.Equal(2, content.Page);
        Assert.Equal(1, content.PageSize);
        Assert.Equal(3, content.TotalCount);
        Assert.Equal(3, content.TotalPages);

        var organization = Assert.Single(content.Items);

        Assert.Equal($"{prefix} B", organization.Name);
    }

    [Fact]
    public async Task List_ShouldReturnBadRequest_WhenPaginationIsInvalid()
    {
        var response = await _client.GetAsync(
            "/api/organizations?page=0&pageSize=20");

        var problem = await response.Content
            .ReadFromJsonAsync<ProblemDetails>();

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.NotNull(problem);
        Assert.Equal("Invalid request", problem.Title);
    }
}
