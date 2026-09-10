using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace SistemaPresenca.Api.Handlers;

public sealed class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var traceId = httpContext.TraceIdentifier;

        var code = exception switch
        {
            BadHttpRequestException => HttpStatusCode.BadRequest,
            UnauthorizedAccessException => HttpStatusCode.Unauthorized,
            HttpRequestException => HttpStatusCode.ServiceUnavailable,
            _ => HttpStatusCode.InternalServerError
        };

        logger.LogError(exception,
            "Unhandled exception ({TraceId}) at {Path}. Message: {Message}",
            traceId,
            httpContext.Request.Path,
            exception.Message);

        var problemDetails = new ProblemDetails
        {
            Status = (int)code,
            Title = "An unexpected error occurred.",
            Detail = "Internal server error. Contact support with the provided traceId.",
            Instance = httpContext.Request.Path,
            Extensions =
                {
                    ["traceId"] = traceId
                }
        };

        httpContext.Response.StatusCode = (int)code;
        httpContext.Response.ContentType = "application/json";

        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }
}
