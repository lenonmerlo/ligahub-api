using System.Net;
using System.Net.Http.Json;
using LigaHub.Api.Contracts.Organizations;
using LigaHub.Api.Contracts.Players;
using LigaHub.Api.Contracts.Teams;
using LigaHub.Api.FunctionalTests;
using LigaHub.Api.FunctionalTests.Database;
using Microsoft.AspNetCore.Mvc;

namespace LigaHub.Api.FunctionalTests.Players;

public sealed class CreatePlayerEndpointTests
    : IClassFixture<LigaHubApiFactory>
{
    private readonly HttpClient _client;

    public CreatePlayerEndpointTests(
        LigaHubApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Create_ShouldReturnCreated_WhenRequestIsValid()
    {
        var organization = await CreateOrganizationAsync();
        var team = await CreateTeamAsync(organization.Id);
        var request = new CreatePlayerRequest(
            $"Jogador {Guid.NewGuid():N}");

        var response = await _client.PostAsJsonAsync(
            $"/api/organizations/{organization.Id}/teams/{team.Id}/players",
            request);

        var content = await response.Content
            .ReadFromJsonAsync<CreatePlayerResponse>();

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(content);
        Assert.NotEqual(Guid.Empty, content.Id);
        Assert.Equal(team.Id, content.TeamId);
        Assert.Equal(request.Name, content.Name);
        Assert.Equal(
            $"/api/organizations/{organization.Id}/teams/{team.Id}/players/{content.Id}",
            response.Headers.Location?.ToString());
    }

    [Fact]
    public async Task Create_ShouldReturnNotFound_WhenTeamDoesNotExist()
    {
        var organization = await CreateOrganizationAsync();
        var teamId = Guid.NewGuid();
        var request = new CreatePlayerRequest(
            $"Jogador {Guid.NewGuid():N}");

        var response = await _client.PostAsJsonAsync(
            $"/api/organizations/{organization.Id}/teams/{teamId}/players",
            request);

        var problem = await response.Content
            .ReadFromJsonAsync<ProblemDetails>();

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.NotNull(problem);
        Assert.Equal("Team not found", problem.Title);
    }

    [Fact]
    public async Task Create_ShouldReturnNotFound_WhenTeamBelongsToAnotherOrganization()
    {
        var firstOrganization = await CreateOrganizationAsync();
        var secondOrganization = await CreateOrganizationAsync();
        var team = await CreateTeamAsync(firstOrganization.Id);
        var request = new CreatePlayerRequest(
            $"Jogador {Guid.NewGuid():N}");

        var response = await _client.PostAsJsonAsync(
            $"/api/organizations/{secondOrganization.Id}/teams/{team.Id}/players",
            request);

        var problem = await response.Content
            .ReadFromJsonAsync<ProblemDetails>();

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.NotNull(problem);
        Assert.Equal("Team not found", problem.Title);
    }

    [Fact]
    public async Task Create_ShouldReturnBadRequest_WhenNameIsInvalid()
    {
        var organization = await CreateOrganizationAsync();
        var team = await CreateTeamAsync(organization.Id);
        var request = new CreatePlayerRequest(" ");

        var response = await _client.PostAsJsonAsync(
            $"/api/organizations/{organization.Id}/teams/{team.Id}/players",
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
        var team = await CreateTeamAsync(organization.Id);
        var request = new CreatePlayerRequest(
            $"Jogador {Guid.NewGuid():N}");

        var firstResponse = await _client.PostAsJsonAsync(
            $"/api/organizations/{organization.Id}/teams/{team.Id}/players",
            request);

        var secondResponse = await _client.PostAsJsonAsync(
            $"/api/organizations/{organization.Id}/teams/{team.Id}/players",
            request);

        var problem = await secondResponse.Content
            .ReadFromJsonAsync<ProblemDetails>();

        Assert.Equal(HttpStatusCode.Created, firstResponse.StatusCode);
        Assert.Equal(HttpStatusCode.Conflict, secondResponse.StatusCode);
        Assert.NotNull(problem);
        Assert.Equal("Player name conflict", problem.Title);
    }

    [Fact]
    public async Task Create_ShouldAllowSameName_InDifferentTeams()
    {
        var organization = await CreateOrganizationAsync();
        var firstTeam = await CreateTeamAsync(organization.Id);
        var secondTeam = await CreateTeamAsync(organization.Id);
        var request = new CreatePlayerRequest(
            $"Jogador {Guid.NewGuid():N}");

        var firstResponse = await _client.PostAsJsonAsync(
            $"/api/organizations/{organization.Id}/teams/{firstTeam.Id}/players",
            request);

        var secondResponse = await _client.PostAsJsonAsync(
            $"/api/organizations/{organization.Id}/teams/{secondTeam.Id}/players",
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
