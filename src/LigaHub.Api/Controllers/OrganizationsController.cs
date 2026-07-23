using LigaHub.Api.Contracts.Organizations;
using LigaHub.Application.Organizations.CreateOrganization;
using Microsoft.AspNetCore.Mvc;

namespace LigaHub.Api.Controllers;

[ApiController]
[Route("api/organizations")]
public sealed class OrganizationsController : ControllerBase
{
    private readonly CreateOrganizationUseCase _useCase;

    public OrganizationsController(
        CreateOrganizationUseCase useCase)
    {
        _useCase = useCase
            ?? throw new ArgumentNullException(nameof(useCase));
    }

    [HttpPost]
    [ProducesResponseType<CreateOrganizationResponse>(
        StatusCodes.Status201Created)]
    [ProducesResponseType<ProblemDetails>(
        StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(
        StatusCodes.Status409Conflict)]
    public async Task<ActionResult<CreateOrganizationResponse>> CreateAsync(
        [FromBody] CreateOrganizationRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateOrganizationCommand(request.Name);

        var result = await _useCase.ExecuteAsync(
            command,
            cancellationToken);

        var response = new CreateOrganizationResponse(
            result.Id,
            result.Name);

        return Created(
            $"/api/organizations/{response.Id}",
            response);
    }
}
