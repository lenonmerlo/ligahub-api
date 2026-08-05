using System.Net;
using System.Net.Http.Json;
using LigaHub.Api.Contracts.Organizations;
using LigaHub.Api.Contracts.Teams;
using LigaHub.Api.FunctionalTests;
using LigaHub.Api.FunctionalTests.Database;
using Microsoft.AspNetCore.Mvc;

namespace LigaHub.Api.FunctionalTests.Teams;

public sealed class ListTeamsEndpointTests
    : IClassFixture<LigaHubApiFactory>
{
    private readonly HttpClient _client;

    public ListTeamsEndpointTests(
        LigaHubApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task List_ShouldReturnPaginatedTeams()
    {
        var organization = await CreateOrganizationAsync();
        var firstTeam = await CreateTeamAsync(
            organization.Id,
            $"A {Guid.NewGuid():N}");
        var secondTeam = await CreateTeamAsync(
            organization.Id,
            $"B {Guid.NewGuid():N}");

        await CreateTeamAsync(
            organization.Id,
            $"C {Guid.NewGuid():N}");

        var response = await _client.GetAsync(
            $"/api/organizations/{organization.Id}/teams?page=1&pageSize=2");

        var content = await response.Content
            .ReadFromApiJsonAsync<ListTeamsResponse>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(content);
        Assert.Equal(1, content.Page);
        Assert.Equal(2, content.PageSize);
        Assert.Equal(3, content.TotalCount);
        Assert.Equal(2, content.TotalPages);
        Assert.Collection(
            content.Items,
            item =>
            {
                Assert.Equal(firstTeam.Id, item.Id);
                Assert.Equal(firstTeam.Name, item.Name);
            },
            item =>
            {
                Assert.Equal(secondTeam.Id, item.Id);
                Assert.Equal(secondTeam.Name, item.Name);
                Assert.Equal(secondTeam.Sport, item.Sport);
            });
    }

    [Fact]
    public async Task List_ShouldReturnEmpty_WhenOrganizationHasNoTeams()
    {
        var organization = await CreateOrganizationAsync();

        var response = await _client.GetAsync(
            $"/api/organizations/{organization.Id}/teams");

        var content = await response.Content
            .ReadFromApiJsonAsync<ListTeamsResponse>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(content);
        Assert.Empty(content.Items);
        Assert.Equal(0, content.TotalCount);
        Assert.Equal(0, content.TotalPages);
    }

    [Fact]
    public async Task List_ShouldReturnNotFound_WhenOrganizationDoesNotExist()
    {
        var organizationId = Guid.NewGuid();

        var response = await _client.GetAsync(
            $"/api/organizations/{organizationId}/teams");

        var problem = await response.Content
            .ReadFromJsonAsync<ProblemDetails>();

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.NotNull(problem);
        Assert.Equal("Organization not found", problem.Title);
    }

    [Fact]
    public async Task List_ShouldReturnBadRequest_WhenPageIsInvalid()
    {
        var organization = await CreateOrganizationAsync();

        var response = await _client.GetAsync(
            $"/api/organizations/{organization.Id}/teams?page=0");

        var problem = await response.Content
            .ReadFromJsonAsync<ProblemDetails>();

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.NotNull(problem);
        Assert.Equal("Invalid request", problem.Title);
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
        Guid organizationId,
        string name)
    {
        var request = new CreateTeamRequest(name);

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
