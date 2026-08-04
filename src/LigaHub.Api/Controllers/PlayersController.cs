using LigaHub.Api.Contracts.Players;
using LigaHub.Application.Players.CreatePlayer;
using Microsoft.AspNetCore.Mvc;

namespace LigaHub.Api.Controllers;

[ApiController]
[Route("api/organizations/{organizationId:guid}/teams/{teamId:guid}/players")]
public sealed class PlayersController : ControllerBase
{
    private readonly CreatePlayerUseCase _createUseCase;

    public PlayersController(
        CreatePlayerUseCase createUseCase)
    {
        _createUseCase = createUseCase
            ?? throw new ArgumentNullException(nameof(createUseCase));
    }

    [HttpPost]
    [ProducesResponseType<CreatePlayerResponse>(
        StatusCodes.Status201Created)]
    [ProducesResponseType<ProblemDetails>(
        StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(
        StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(
        StatusCodes.Status409Conflict)]
    public async Task<ActionResult<CreatePlayerResponse>> CreateAsync(
        Guid organizationId,
        Guid teamId,
        [FromBody] CreatePlayerRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreatePlayerCommand(
            organizationId,
            teamId,
            request.Name);

        var result = await _createUseCase.ExecuteAsync(
            command,
            cancellationToken);

        if (result is null)
        {
            return Problem(
                statusCode: StatusCodes.Status404NotFound,
                title: "Team not found",
                detail: $"Team '{teamId}' was not found in organization '{organizationId}'.");
        }

        var response = new CreatePlayerResponse(
            result.Id,
            result.TeamId,
            result.Name);

        return Created(
            $"/api/organizations/{organizationId}/teams/{teamId}/players/{response.Id}",
            response);
    }
}
