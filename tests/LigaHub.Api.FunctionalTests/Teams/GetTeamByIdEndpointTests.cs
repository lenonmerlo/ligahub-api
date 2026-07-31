using System.Net;
using System.Net.Http.Json;
using LigaHub.Api.Contracts.Organizations;
using LigaHub.Api.Contracts.Teams;
using LigaHub.Api.FunctionalTests.Database;
using Microsoft.AspNetCore.Mvc;

namespace LigaHub.Api.FunctionalTests.Teams;

public sealed class GetTeamByIdEndpointTests
    : IClassFixture<LigaHubApiFactory>
{
    private readonly HttpClient _client;

    public GetTeamByIdEndpointTests(
        LigaHubApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetById_ShouldReturnOk_WhenTeamExists()
    {
        var organization = await CreateOrganizationAsync();
        var team = await CreateTeamAsync(organization.Id);

        var response = await _client.GetAsync(
            $"/api/organizations/{organization.Id}/teams/{team.Id}");

        var content = await response.Content
            .ReadFromJsonAsync<GetTeamByIdResponse>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(content);
        Assert.Equal(team.Id, content.Id);
        Assert.Equal(organization.Id, content.OrganizationId);
        Assert.Equal(team.Name, content.Name);
    }

    [Fact]
    public async Task GetById_ShouldReturnNotFound_WhenTeamDoesNotExist()
    {
        var organization = await CreateOrganizationAsync();
        var teamId = Guid.NewGuid();

        var response = await _client.GetAsync(
            $"/api/organizations/{organization.Id}/teams/{teamId}");

        var problem = await response.Content
            .ReadFromJsonAsync<ProblemDetails>();

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.NotNull(problem);
        Assert.Equal("Team not found", problem.Title);
    }

    [Fact]
    public async Task GetById_ShouldReturnNotFound_WhenTeamBelongsToAnotherOrganization()
    {
        var firstOrganization = await CreateOrganizationAsync();
        var secondOrganization = await CreateOrganizationAsync();
        var team = await CreateTeamAsync(firstOrganization.Id);

        var response = await _client.GetAsync(
            $"/api/organizations/{secondOrganization.Id}/teams/{team.Id}");

        var problem = await response.Content
            .ReadFromJsonAsync<ProblemDetails>();

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.NotNull(problem);
        Assert.Equal("Team not found", problem.Title);
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
            .ReadFromJsonAsync<CreateTeamResponse>();

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(content);

        return content;
    }
}
