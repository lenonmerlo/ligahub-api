using LigaHub.Api.Contracts.Organizations;
using LigaHub.Application.Organizations.CreateOrganization;
using LigaHub.Application.Organizations.GetOrganizationById;
using LigaHub.Application.Organizations.ListOrganizations;
using LigaHub.Application.Organizations.UpdateOrganizationName;
using Microsoft.AspNetCore.Mvc;

namespace LigaHub.Api.Controllers;

[ApiController]
[Route("api/organizations")]
public sealed class OrganizationsController : ControllerBase
{
    private readonly CreateOrganizationUseCase _useCase;
    private readonly GetOrganizationByIdUseCase _getByIdUseCase;
    private readonly ListOrganizationsUseCase _listUseCase;
    private readonly UpdateOrganizationNameUseCase _updateNameUseCase;

    public OrganizationsController(
        CreateOrganizationUseCase useCase,
        GetOrganizationByIdUseCase getByIdUseCase,
        ListOrganizationsUseCase listUseCase,
        UpdateOrganizationNameUseCase updateNameUse)
    {
        _useCase = useCase
            ?? throw new ArgumentNullException(nameof(useCase));

        _getByIdUseCase = getByIdUseCase
            ?? throw new ArgumentNullException(nameof(getByIdUseCase));

        _listUseCase = listUseCase
            ?? throw new ArgumentNullException(nameof(listUseCase));

        _updateNameUseCase = updateNameUse
            ?? throw new ArgumentNullException(nameof(updateNameUse));
    }

    [HttpGet]
    [ProducesResponseType<ListOrganizationsResponse>(
        StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(
        StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ListOrganizationsResponse>> ListAsync(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var query = new ListOrganizationsQuery(
            page,
            pageSize);

        var result = await _listUseCase.ExecuteAsync(
            query,
            cancellationToken);

        var items = result.Items
            .Select(item => new OrganizationListItemResponse(
                item.Id,
                item.Name))
            .ToArray();

        return Ok(new ListOrganizationsResponse(
            items,
            result.Page,
            result.PageSize,
            result.TotalCount,
            result.TotalPages));
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

    [HttpPut("{id:guid}")]
    [ProducesResponseType<UpdateOrganizationNameResponse>(
        StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(
        StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(
        StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(
        StatusCodes.Status409Conflict)]
    public async Task<ActionResult<UpdateOrganizationNameResponse>> UpdateNameAsync(
        Guid id,
        [FromBody] UpdateOrganizationNameRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateOrganizationNameCommand(
            id,
            request.Name);

        var result = await _updateNameUseCase.ExecuteAsync(
            command,
            cancellationToken);

        if (result is null)
        {
            return Problem(
                statusCode: StatusCodes.Status404NotFound,
                title: "Organization not found",
                detail: $"Organization '{id}' was not found.");
        }

        return Ok(new UpdateOrganizationNameResponse(
            result.Id,
            result.Name));
    }
}
