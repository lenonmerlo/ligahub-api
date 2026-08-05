using System.Net;
using System.Net.Http.Json;
using LigaHub.Api.Contracts.Organizations;
using LigaHub.Api.Contracts.Teams;
using LigaHub.Api.FunctionalTests.Database;
using LigaHub.Api.FunctionalTests;
using LigaHub.Domain.Teams;
using Microsoft.AspNetCore.Mvc;

namespace LigaHub.Api.FunctionalTests.Teams;

public sealed class CreateTeamEndpointTests
    : IClassFixture<LigaHubApiFactory>
{
    private readonly HttpClient _client;

    public CreateTeamEndpointTests(
        LigaHubApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Create_ShouldReturnCreated_WhenRequestIsValid()
    {
        var organization = await CreateOrganizationAsync();
        var request = new CreateTeamRequest(
            $"Time {Guid.NewGuid():N}",
            Sport.Basketball);

        var response = await _client.PostAsJsonAsync(
            $"/api/organizations/{organization.Id}/teams",
            request);

        var content = await response.Content
            .ReadFromApiJsonAsync<CreateTeamResponse>();

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(content);
        Assert.NotEqual(Guid.Empty, content.Id);
        Assert.Equal(organization.Id, content.OrganizationId);
        Assert.Equal(request.Name, content.Name);
        Assert.Equal(Sport.Basketball, content.Sport);
        Assert.Equal(
            $"/api/organizations/{organization.Id}/teams/{content.Id}",
            response.Headers.Location?.ToString());
    }

    [Fact]
    public async Task Create_ShouldReturnNotFound_WhenOrganizationDoesNotExist()
    {
        var organizationId = Guid.NewGuid();
        var request = new CreateTeamRequest(
            $"Time {Guid.NewGuid():N}");

        var response = await _client.PostAsJsonAsync(
            $"/api/organizations/{organizationId}/teams",
            request);

        var problem = await response.Content
            .ReadFromJsonAsync<ProblemDetails>();

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.NotNull(problem);
        Assert.Equal("Organization not found", problem.Title);
    }

    [Fact]
    public async Task Create_ShouldReturnBadRequest_WhenNameIsInvalid()
    {
        var organization = await CreateOrganizationAsync();
        var request = new CreateTeamRequest(" ");

        var response = await _client.PostAsJsonAsync(
            $"/api/organizations/{organization.Id}/teams",
            request);

        var problem = await response.Content
            .ReadFromJsonAsync<ProblemDetails>();

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.NotNull(problem);
        Assert.Equal("Invalid request", problem.Title);
    }

    [Fact]
    public async Task Create_ShouldReturnConflict_WhenNameAlreadyExists()
    {
        var organization = await CreateOrganizationAsync();
        var request = new CreateTeamRequest(
            $"Time {Guid.NewGuid():N}");

        var firstResponse = await _client.PostAsJsonAsync(
            $"/api/organizations/{organization.Id}/teams",
            request);

        var secondResponse = await _client.PostAsJsonAsync(
            $"/api/organizations/{organization.Id}/teams",
            request);

        var problem = await secondResponse.Content
            .ReadFromJsonAsync<ProblemDetails>();

        Assert.Equal(HttpStatusCode.Created, firstResponse.StatusCode);
        Assert.Equal(HttpStatusCode.Conflict, secondResponse.StatusCode);
        Assert.NotNull(problem);
        Assert.Equal("Team name conflict", problem.Title);
    }

    [Fact]
    public async Task Create_ShouldAllowSameName_InDifferentOrganizations()
    {
        var firstOrganization = await CreateOrganizationAsync();
        var secondOrganization = await CreateOrganizationAsync();
        var request = new CreateTeamRequest(
            $"Time {Guid.NewGuid():N}");

        var firstResponse = await _client.PostAsJsonAsync(
            $"/api/organizations/{firstOrganization.Id}/teams",
            request);

        var secondResponse = await _client.PostAsJsonAsync(
            $"/api/organizations/{secondOrganization.Id}/teams",
            request);

        Assert.Equal(HttpStatusCode.Created, firstResponse.StatusCode);
        Assert.Equal(HttpStatusCode.Created, secondResponse.StatusCode);
    }

    private async Task<CreateOrganizationResponse> CreateOrganizationAsync()
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

        return content;
    }
}
