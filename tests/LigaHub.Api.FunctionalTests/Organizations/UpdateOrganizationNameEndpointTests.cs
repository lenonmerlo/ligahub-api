using System.Net;
using System.Net.Http.Json;
using LigaHub.Api.Contracts.Organizations;
using LigaHub.Api.FunctionalTests.Database;
using Microsoft.AspNetCore.Mvc;

namespace LigaHub.Api.FunctionalTests.Organizations;

public sealed class UpdateOrganizationNameEndpointTests
    : IClassFixture<LigaHubApiFactory>
{
    private readonly HttpClient _client;

    public UpdateOrganizationNameEndpointTests(
        LigaHubApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task UpdateName_ShouldReturnOk_WhenRequestIsValid()
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

        var updateRequest = new UpdateOrganizationNameRequest(
            $"Liga Atualizada {Guid.NewGuid():N}");

        var response = await _client.PutAsJsonAsync(
            $"/api/organizations/{createdOrganization.Id}",
            updateRequest);

        var content = await response.Content
            .ReadFromJsonAsync<UpdateOrganizationNameResponse>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(content);
        Assert.Equal(createdOrganization.Id, content.Id);
        Assert.Equal(updateRequest.Name, content.Name);
    }

    [Fact]
    public async Task UpdateName_ShouldReturnNotFound_WhenOrganizationDoesNotExist()
    {
        var id = Guid.NewGuid();
        var request = new UpdateOrganizationNameRequest(
            $"Liga {Guid.NewGuid():N}");

        var response = await _client.PutAsJsonAsync(
            $"/api/organizations/{id}",
            request);

        var problem = await response.Content
            .ReadFromJsonAsync<ProblemDetails>();

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.NotNull(problem);
        Assert.Equal("Organization not found", problem.Title);
    }

    [Fact]
    public async Task UpdateName_ShouldReturnBadRequest_WhenNameIsInvalid()
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

        var updateRequest = new UpdateOrganizationNameRequest(" ");

        var response = await _client.PutAsJsonAsync(
            $"/api/organizations/{createdOrganization.Id}",
            updateRequest);

        var problem = await response.Content
            .ReadFromJsonAsync<ProblemDetails>();

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.NotNull(problem);
        Assert.Equal("Invalid request", problem.Title);
    }

    [Fact]
    public async Task UpdateName_ShouldReturnConflict_WhenNameAlreadyExists()
    {
        var firstRequest = new CreateOrganizationRequest(
            $"Liga {Guid.NewGuid():N}");

        var firstResponse = await _client.PostAsJsonAsync(
            "/api/organizations",
            firstRequest);

        var firstOrganization = await firstResponse.Content
            .ReadFromJsonAsync<CreateOrganizationResponse>();

        var secondRequest = new CreateOrganizationRequest(
            $"Liga {Guid.NewGuid():N}");

        var secondResponse = await _client.PostAsJsonAsync(
            "/api/organizations",
            secondRequest);

        Assert.Equal(HttpStatusCode.Created, firstResponse.StatusCode);
        Assert.Equal(HttpStatusCode.Created, secondResponse.StatusCode);
        Assert.NotNull(firstOrganization);

        var updateRequest = new UpdateOrganizationNameRequest(
            secondRequest.Name);

        var response = await _client.PutAsJsonAsync(
            $"/api/organizations/{firstOrganization.Id}",
            updateRequest);

        var problem = await response.Content
            .ReadFromJsonAsync<ProblemDetails>();

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        Assert.NotNull(problem);
        Assert.Equal("Organization name conflict", problem.Title);
    }
}
