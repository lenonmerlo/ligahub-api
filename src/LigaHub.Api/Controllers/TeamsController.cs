using LigaHub.Api.Contracts.Teams;
using LigaHub.Application.Teams.CreateTeam;
using Microsoft.AspNetCore.Mvc;

namespace LigaHub.Api.Controllers;

[ApiController]
[Route("api/organizations/{organizationId:guid}/teams")]
public sealed class TeamsController : ControllerBase 
{
    private readonly CreateTeamUseCase _createUseCase;

    public TeamsController(CreateTeamUseCase createUseCase)
    {
        _createUseCase = createUseCase
            ?? throw new ArgumentNullException(nameof(createUseCase));
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
