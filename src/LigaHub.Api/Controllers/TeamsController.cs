using LigaHub.Api.Contracts.Teams;
using LigaHub.Application.Teams.CreateTeam;
using LigaHub.Application.Teams.GetTeamById;
using LigaHub.Application.Teams.ListTeams;
using Microsoft.AspNetCore.Mvc;

namespace LigaHub.Api.Controllers;

[ApiController]
[Route("api/organizations/{organizationId:guid}/teams")]
public sealed class TeamsController : ControllerBase
{
    private readonly CreateTeamUseCase _createUseCase;
    private readonly GetTeamByIdUseCase _getByIdUseCase;
    private readonly ListTeamsUseCase _listUseCase;

    public TeamsController(
        CreateTeamUseCase createUseCase,
        GetTeamByIdUseCase getTeamByIdUse,
        ListTeamsUseCase listUseCase)
    {
        _createUseCase = createUseCase
            ?? throw new ArgumentNullException(nameof(createUseCase));

        _getByIdUseCase = getTeamByIdUse
            ?? throw new ArgumentNullException(nameof(getTeamByIdUse));

        _listUseCase = listUseCase
            ?? throw new ArgumentNullException(nameof(listUseCase));
    }

    [HttpGet]
    [ProducesResponseType<ListTeamsResponse>(
        StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(
        StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(
        StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ListTeamsResponse>> ListAsync(
        Guid organizationId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var query = new ListTeamsQuery(
            organizationId,
            page,
            pageSize);

        var result = await _listUseCase.ExecuteAsync(
            query,
            cancellationToken);

        if (result is null)
        {
            return Problem(
                statusCode: StatusCodes.Status404NotFound,
                title: "Organization not found",
                detail: $"Organization '{organizationId}' was not found.");
        }

        var items = result.Items
            .Select(item => new TeamListItemResponse(
                item.Id,
                item.Name))
            .ToArray();

        return Ok(new ListTeamsResponse(
            items,
            result.Page,
            result.PageSize,
            result.TotalCount,
            result.TotalPages));
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType<GetTeamByIdResponse>(
        StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(
        StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GetTeamByIdResponse>> GetByIdAsync(
        Guid organizationId,
        Guid id,
        CancellationToken cancellationToken)
    {
        var query = new GetTeamByIdQuery(
            organizationId,
            id);

        var result = await _getByIdUseCase.ExecuteAsync(
            query,
            cancellationToken);

        if (result is null)
        {
            return Problem(
                statusCode: StatusCodes.Status404NotFound,
                title: "Team not found",
                detail: $"Team '{id}' was not found in organization '{organizationId}'.");
        }

        return Ok(new GetTeamByIdResponse(
            result.Id,
            result.OrganizationId,
            result.Name));
    }

    [HttpPost]
    [ProducesResponseType<CreateTeamResponse>(
        StatusCodes.Status201Created)]
    [ProducesResponseType<ProblemDetails>(
        StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(
        StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(
        StatusCodes.Status409Conflict)]
    public async Task<ActionResult<CreateTeamResponse>> CreateAsync(
        Guid organizationId,
        [FromBody] CreateTeamRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateTeamCommand(
            organizationId,
            request.Name);

        var result = await _createUseCase.ExecuteAsync(
            command,
            cancellationToken);

        if (result is null)
        {
            return Problem(
                statusCode: StatusCodes.Status404NotFound,
                title: "Organization not found",
                detail: $"Organization '{organizationId}' was not found.");
        }

        var response = new CreateTeamResponse(
            result.Id,
            result.OrganizationId,
            result.Name);

        return Created(
            $"/api/organizations/{organizationId}/teams/{response.Id}",
            response);
    }
}
