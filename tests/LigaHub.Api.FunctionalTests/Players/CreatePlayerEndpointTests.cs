using System.Net;
using System.Net.Http.Json;
using LigaHub.Api.Contracts.Organizations;
using LigaHub.Api.Contracts.Players;
using LigaHub.Api.Contracts.Teams;
using LigaHub.Api.FunctionalTests;
using LigaHub.Api.FunctionalTests.Database;
using LigaHub.Domain.Players;
using Microsoft.AspNetCore.Mvc;

namespace LigaHub.Api.FunctionalTests.Players;

public sealed class CreatePlayerEndpointTests
    : IClassFixture<LigaHubApiFactory>
{
    private static readonly DateOnly ValidBirthDate =
        new(2000, 1, 1);

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
            $"Jogador {Guid.NewGuid():N}",
            ValidBirthDate,
            Sex.Male,
            10);

        var response = await _client.PostAsJsonAsync(
            $"/api/organizations/{organization.Id}/teams/{team.Id}/players",
            request);

        var content = await response.Content
            .ReadFromApiJsonAsync<CreatePlayerResponse>();

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(content);
        Assert.NotEqual(Guid.Empty, content.Id);
        Assert.Equal(team.Id, content.TeamId);
        Assert.Equal(request.Name, content.Name);
        Assert.Equal(request.BirthDate, content.BirthDate);
        Assert.Equal(request.Sex, content.Sex);
        Assert.Equal(request.JerseyNumber, content.JerseyNumber);
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
            $"Jogador {Guid.NewGuid():N}",
            ValidBirthDate,
            Sex.Female,
            7);

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
            $"Jogador {Guid.NewGuid():N}",
            ValidBirthDate,
            Sex.Male,
            9);

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
        var request = new CreatePlayerRequest(
            " ",
            ValidBirthDate,
            Sex.Male,
            11);

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
    public async Task Create_ShouldReturnConflict_WhenJerseyNumberAlreadyExists()
    {
        var organization = await CreateOrganizationAsync();
        var team = await CreateTeamAsync(organization.Id);
        var firstRequest = new CreatePlayerRequest(
            $"Jogador A {Guid.NewGuid():N}",
            ValidBirthDate,
            Sex.Male,
            10);
        var secondRequest = new CreatePlayerRequest(
            $"Jogador B {Guid.NewGuid():N}",
            new DateOnly(2001, 2, 2),
            Sex.Female,
            10);

        var firstResponse = await _client.PostAsJsonAsync(
            $"/api/organizations/{organization.Id}/teams/{team.Id}/players",
            firstRequest);

        var secondResponse = await _client.PostAsJsonAsync(
            $"/api/organizations/{organization.Id}/teams/{team.Id}/players",
            secondRequest);

        var problem = await secondResponse.Content
            .ReadFromJsonAsync<ProblemDetails>();

        Assert.Equal(HttpStatusCode.Created, firstResponse.StatusCode);
        Assert.Equal(HttpStatusCode.Conflict, secondResponse.StatusCode);
        Assert.NotNull(problem);
        Assert.Equal(
            "Player jersey number conflict",
            problem.Title);
        Assert.Contains("10", problem.Detail);
    }

    [Fact]
    public async Task Create_ShouldAllowSameName_WithDifferentJerseyNumbers()
    {
        var organization = await CreateOrganizationAsync();
        var team = await CreateTeamAsync(organization.Id);
        var playerName = $"Jogador {Guid.NewGuid():N}";
        var firstRequest = new CreatePlayerRequest(
            playerName,
            ValidBirthDate,
            Sex.Male,
            8);
        var secondRequest = new CreatePlayerRequest(
            playerName,
            new DateOnly(2001, 2, 2),
            Sex.Female,
            9);

        var firstResponse = await _client.PostAsJsonAsync(
            $"/api/organizations/{organization.Id}/teams/{team.Id}/players",
            firstRequest);

        var secondResponse = await _client.PostAsJsonAsync(
            $"/api/organizations/{organization.Id}/teams/{team.Id}/players",
            secondRequest);

        Assert.Equal(HttpStatusCode.Created, firstResponse.StatusCode);
        Assert.Equal(HttpStatusCode.Created, secondResponse.StatusCode);
    }

    [Fact]
    public async Task Create_ShouldAllowSameJerseyNumber_InDifferentTeams()
    {
        var organization = await CreateOrganizationAsync();
        var firstTeam = await CreateTeamAsync(organization.Id);
        var secondTeam = await CreateTeamAsync(organization.Id);
        var firstRequest = new CreatePlayerRequest(
            $"Jogador A {Guid.NewGuid():N}",
            ValidBirthDate,
            Sex.Male,
            12);
        var secondRequest = new CreatePlayerRequest(
            $"Jogador B {Guid.NewGuid():N}",
            new DateOnly(2001, 2, 2),
            Sex.Female,
            12);

        var firstResponse = await _client.PostAsJsonAsync(
            $"/api/organizations/{organization.Id}/teams/{firstTeam.Id}/players",
            firstRequest);

        var secondResponse = await _client.PostAsJsonAsync(
            $"/api/organizations/{organization.Id}/teams/{secondTeam.Id}/players",
            secondRequest);

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
