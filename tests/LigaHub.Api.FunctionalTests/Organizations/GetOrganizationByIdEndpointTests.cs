using System.Net;
using System.Net.Http.Json;
using LigaHub.Api.Contracts.Organizations;
using LigaHub.Api.FunctionalTests.Database;
using Microsoft.AspNetCore.Mvc;

namespace LigaHub.Api.FunctionalTests.Organizations;

public sealed class GetOrganizationByIdEndpointTests : IClassFixture<LigaHubApiFactory>
{
    private readonly HttpClient _client;

    public GetOrganizationByIdEndpointTests(
        LigaHubApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetById_ShouldReturnOk_WhenOrganizationExists()
    {
        var createRequest = new CreateOrganizationRequest(
            $"Liga {Guid.NewGuid():N}");

        var createResponse = await _client.PostAsJsonAsync(
            "/api/organizations",
            createRequest);

        var createdOrganization = await createResponse.Content
            .ReadFromJsonAsync<CreateOrganizationResponse>();

        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        Assert.NotNull(createdOrganization);

        var response = await _client.GetAsync(
            $"/api/organizations/{createdOrganization.Id}");

        var content = await response.Content
            .ReadFromJsonAsync<GetOrganizationByIdResponse>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(content);
        Assert.Equal(createdOrganization.Id, content.Id);
        Assert.Equal(createdOrganization.Name, content.Name);
    }

    [Fact]
    public async Task GetById_ShouldReturnNotFound_WhenOrganizationDoesNotExists()
    {
        var id = Guid.NewGuid();

        var response = await _client.GetAsync(
            $"/api/organizations/{id}");

        var problem = await response.Content
            .ReadFromJsonAsync<ProblemDetails>();

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.NotNull(problem);
        Assert.Equal("Organization not found", problem.Title);
    }
}
