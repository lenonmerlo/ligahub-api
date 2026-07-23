using LigaHub.Application.Organizations;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace LigaHub.Api.ExceptionHandling;

internal sealed class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(
        ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var (statusCode, title, detail) = exception switch
        {
            OrganizationNameAlreadyExistsException => (
            StatusCodes.Status409Conflict,
            "Organization name conflict",
            exception.Message),

            ArgumentException => (
            StatusCodes.Status400BadRequest,
            "Invlaid request",
            exception.Message),

            _ => (
            StatusCodes.Status500InternalServerError,
            "Internal server error",
            "An unexpected error occurred.")
        };

        if (statusCode >= StatusCodes.Status500InternalServerError)
        {
            _logger.LogError(
                exception,
                "An unhandled exception occurred.");
        }
        else
        {
            _logger.LogWarning(
                exception,
                "An request could not be processed.");
        }

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = detail,
            Instance = httpContext.Request.Path
        };

        httpContext.Response.StatusCode = statusCode;

        await httpContext.Response.WriteAsJsonAsync(
            problemDetails,
            cancellationToken);

        return true;
    }
}
