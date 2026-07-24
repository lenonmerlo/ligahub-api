using LigaHub.Api.Contracts.Organizations;
using LigaHub.Application.Organizations.CreateOrganization;
using LigaHub.Application.Organizations.GetOrganizationById;
using Microsoft.AspNetCore.Mvc;

namespace LigaHub.Api.Controllers;

[ApiController]
[Route("api/organizations")]
public sealed class OrganizationsController : ControllerBase
{
    private readonly CreateOrganizationUseCase _useCase;
    private readonly GetOrganizationByIdUseCase _getByIdUseCase;

    public OrganizationsController(
        CreateOrganizationUseCase useCase,
        GetOrganizationByIdUseCase getByIdUseCase)
    {
        _useCase = useCase
            ?? throw new ArgumentNullException(nameof(useCase));

        _getByIdUseCase = getByIdUseCase
            ?? throw new ArgumentNullException(nameof(getByIdUseCase));
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType<GetOrganizationByIdResponse>(
        StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(
        StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GetOrganizationByIdResponse>> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        var query = new GetOrganizationByIdQuery(id);

        var result = await _getByIdUseCase.ExecuteAsync(
            query,
            cancellationToken);

        if (result is null)
        {
            return Problem(
                statusCode: StatusCodes.Status404NotFound,
                title: "Organization not found",
                detail: $"Organization '{id}' was not found.");
        }

        return Ok(new GetOrganizationByIdResponse(
            result.Id,
            result.Name));
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
