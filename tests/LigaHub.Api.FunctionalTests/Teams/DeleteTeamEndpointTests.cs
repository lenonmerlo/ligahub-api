using System.Net;
using System.Net.Http.Json;
using LigaHub.Api.Contracts.Organizations;
using LigaHub.Api.Contracts.Teams;
using LigaHub.Api.FunctionalTests;
using LigaHub.Api.FunctionalTests.Database;
using Microsoft.AspNetCore.Mvc;

namespace LigaHub.Api.FunctionalTests.Teams;

public sealed class DeleteTeamEndpointTests
    : IClassFixture<LigaHubApiFactory>
{
    private readonly HttpClient _client;

    public DeleteTeamEndpointTests(
        LigaHubApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Delete_ShouldReturnNoContent_WhenTeamExists()
    {
        var organization = await CreateOrganizationAsync();
        var team = await CreateTeamAsync(organization.Id);

        var response = await _client.DeleteAsync(
            $"/api/organizations/{organization.Id}/teams/{team.Id}");

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        var getResponse = await _client.GetAsync(
            $"/api/organizations/{organization.Id}/teams/{team.Id}");

        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    [Fact]
    public async Task Delete_ShouldReturnNotFound_WhenTeamDoesNotExist()
    {
        var organization = await CreateOrganizationAsync();
        var teamId = Guid.NewGuid();

        var response = await _client.DeleteAsync(
            $"/api/organizations/{organization.Id}/teams/{teamId}");

        var problem = await response.Content
            .ReadFromJsonAsync<ProblemDetails>();

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.NotNull(problem);
        Assert.Equal("Team not found", problem.Title);
    }

    [Fact]
    public async Task Delete_ShouldReturnNotFound_WhenTeamBelongsToAnotherOrganization()
    {
        var firstOrganization = await CreateOrganizationAsync();
        var secondOrganization = await CreateOrganizationAsync();
        var team = await CreateTeamAsync(firstOrganization.Id);

        var response = await _client.DeleteAsync(
            $"/api/organizations/{secondOrganization.Id}/teams/{team.Id}");

        var problem = await response.Content
            .ReadFromJsonAsync<ProblemDetails>();

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.NotNull(problem);
        Assert.Equal("Team not found", problem.Title);

        var getResponse = await _client.GetAsync(
            $"/api/organizations/{firstOrganization.Id}/teams/{team.Id}");

        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
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

    private async Task<CreateTeamResponse> CreateTeamAsync(
        Guid organizationId)
    {
        var request = new CreateTeamRequest(
            $"Time {Guid.NewGuid():N}");

        var response = await _client.PostAsJsonAsync(
            $"/api/organizations/{organizationId}/teams",
            request);

        var content = await response.Content
            .ReadFromApiJsonAsync<CreateTeamResponse>();

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(content);

        return content;
    }
}
