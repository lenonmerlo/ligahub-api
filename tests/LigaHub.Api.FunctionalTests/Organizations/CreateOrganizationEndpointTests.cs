using LigaHub.Api.Contracts.Organizations;
using LigaHub.Api.FunctionalTests.Database;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Http.Json;

namespace LigaHub.Api.FunctionalTests.Organizations;

public sealed class CreateOrganizationEndpointTests : IClassFixture<LigaHubApiFactory>
{
    private readonly HttpClient _client;

    public CreateOrganizationEndpointTests(
        LigaHubApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Create_ShouldReturnCreated_WhenRequestIsValid()
    {
        var request = new CreateOrganizationRequest(
            $"Liga {Guid.NewGuid():N}");

        var response = await _client.PostAsJsonAsync(
            "/api/organizations",
            request);

        var content = await response.Content
            .ReadFromJsonAsync<CreateOrganizationResponse>();

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(content);
        Assert.NotEqual(Guid.Empty, content.Id);
        Assert.Equal(request.Name, content.Name);
        Assert.Equal(
            $"/api/organizations/{content.Id}",
            response.Headers.Location?.ToString());
    }

    [Fact]
    public async Task Create_ShouldReturnConflict_WhenNameAlreadyExists()
    {
        var request = new CreateOrganizationRequest(
            $"Liga {Guid.NewGuid():N}");

        var firstResponse = await _client.PostAsJsonAsync(
            "/api/organizations",
            request);

        var secondResponse = await _client.PostAsJsonAsync(
            "/api/organizations",
            request);

        var problem = await secondResponse.Content
            .ReadFromJsonAsync<ProblemDetails>();

        Assert.Equal(HttpStatusCode.Created, firstResponse.StatusCode);
        Assert.Equal(HttpStatusCode.Conflict, secondResponse.StatusCode);
        Assert.NotNull(problem);
        Assert.Equal(
            "Organization name conflict",
            problem.Title);
    }

    [Fact]
    public async Task Create_ShouldReturnBadRequest_WhenNameIsInvalid()
    {
        var request = new CreateOrganizationRequest(" ");

        var response = await _client.PostAsJsonAsync(
            "/api/organizations",
            request);

        var problem = await response.Content
            .ReadFromJsonAsync<ProblemDetails>();

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.NotNull(problem);
        Assert.Equal("Invalid request", problem.Title);
    }
}
