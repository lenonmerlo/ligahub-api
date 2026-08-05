using System.Net;
using System.Net.Http.Json;
using LigaHub.Api.Contracts.Organizations;
using LigaHub.Api.Contracts.Teams;
using LigaHub.Api.FunctionalTests;
using LigaHub.Api.FunctionalTests.Database;
using Microsoft.AspNetCore.Mvc;

namespace LigaHub.Api.FunctionalTests.Teams;

public sealed class UpdateTeamNameEndpointTests
    : IClassFixture<LigaHubApiFactory>
{
    private readonly HttpClient _client;

    public UpdateTeamNameEndpointTests(
        LigaHubApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task UpdateName_ShouldReturnOk_WhenRequestIsValid()
    {
        var organization = await CreateOrganizationAsync();
        var team = await CreateTeamAsync(organization.Id);
        var request = new UpdateTeamNameRequest(
            $"Time Atualizado {Guid.NewGuid():N}");

        var response = await _client.PutAsJsonAsync(
            $"/api/organizations/{organization.Id}/teams/{team.Id}",
            request);

        var content = await response.Content
            .ReadFromApiJsonAsync<UpdateTeamNameResponse>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(content);
        Assert.Equal(team.Id, content.Id);
        Assert.Equal(organization.Id, content.OrganizationId);
        Assert.Equal(request.Name, content.Name);
        Assert.Equal(team.Sport, content.Sport);

        var getResponse = await _client.GetAsync(
            $"/api/organizations/{organization.Id}/teams/{team.Id}");

        var persistedTeam = await getResponse.Content
            .ReadFromApiJsonAsync<GetTeamByIdResponse>();

        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
        Assert.NotNull(persistedTeam);
        Assert.Equal(request.Name, persistedTeam.Name);
        Assert.Equal(team.Sport, persistedTeam.Sport);
    }

    [Fact]
    public async Task UpdateName_ShouldReturnNotFound_WhenTeamDoesNotExist()
    {
        var organization = await CreateOrganizationAsync();
        var teamId = Guid.NewGuid();
        var request = new UpdateTeamNameRequest(
            $"Time {Guid.NewGuid():N}");

        var response = await _client.PutAsJsonAsync(
            $"/api/organizations/{organization.Id}/teams/{teamId}",
            request);

        var problem = await response.Content
            .ReadFromJsonAsync<ProblemDetails>();

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.NotNull(problem);
        Assert.Equal("Team not found", problem.Title);
    }

    [Fact]
    public async Task UpdateName_ShouldReturnNotFound_WhenTeamBelongsToAnotherOrganization()
    {
        var firstOrganization = await CreateOrganizationAsync();
        var secondOrganization = await CreateOrganizationAsync();
        var team = await CreateTeamAsync(firstOrganization.Id);
        var request = new UpdateTeamNameRequest(
            $"Time {Guid.NewGuid():N}");

        var response = await _client.PutAsJsonAsync(
            $"/api/organizations/{secondOrganization.Id}/teams/{team.Id}",
            request);

        var problem = await response.Content
            .ReadFromJsonAsync<ProblemDetails>();

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.NotNull(problem);
        Assert.Equal("Team not found", problem.Title);
    }

    [Fact]
    public async Task UpdateName_ShouldReturnBadRequest_WhenNameIsInvalid()
    {
        var organization = await CreateOrganizationAsync();
        var team = await CreateTeamAsync(organization.Id);
        var request = new UpdateTeamNameRequest(" ");

        var response = await _client.PutAsJsonAsync(
            $"/api/organizations/{organization.Id}/teams/{team.Id}",
            request);

        var problem = await response.Content
            .ReadFromJsonAsync<ProblemDetails>();

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.NotNull(problem);
        Assert.Equal("Invalid request", problem.Title);
    }

    [Fact]
    public async Task UpdateName_ShouldReturnConflict_WhenNameAlreadyExists()
    {
        var organization = await CreateOrganizationAsync();
        var firstTeam = await CreateTeamAsync(organization.Id);
        var secondTeam = await CreateTeamAsync(organization.Id);
        var request = new UpdateTeamNameRequest(secondTeam.Name);

        var response = await _client.PutAsJsonAsync(
            $"/api/organizations/{organization.Id}/teams/{firstTeam.Id}",
            request);

        var problem = await response.Content
            .ReadFromJsonAsync<ProblemDetails>();

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        Assert.NotNull(problem);
        Assert.Equal("Team name conflict", problem.Title);
    }

    [Fact]
    public async Task UpdateName_ShouldAllowSameName_InDifferentOrganizations()
    {
        var firstOrganization = await CreateOrganizationAsync();
        var secondOrganization = await CreateOrganizationAsync();
        var firstTeam = await CreateTeamAsync(firstOrganization.Id);
        var secondTeam = await CreateTeamAsync(secondOrganization.Id);
        var request = new UpdateTeamNameRequest(secondTeam.Name);

        var response = await _client.PutAsJsonAsync(
            $"/api/organizations/{firstOrganization.Id}/teams/{firstTeam.Id}",
            request);

        var content = await response.Content
            .ReadFromApiJsonAsync<UpdateTeamNameResponse>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(content);
        Assert.Equal(firstTeam.Id, content.Id);
        Assert.Equal(firstOrganization.Id, content.OrganizationId);
        Assert.Equal(secondTeam.Name, content.Name);
        Assert.Equal(firstTeam.Sport, content.Sport);
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
