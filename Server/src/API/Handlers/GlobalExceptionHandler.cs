using System.Text.Json;
using API.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;

namespace API.Handlers;

public sealed class GlobalExceptionHandler(
    ILogger<GlobalExceptionHandler> logger,
    IProblemDetailsService problemDetailsService,
    IHostEnvironment environment) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        logger.LogError(exception, "Unhandled exception occurred. TraceId: {TraceId}",
            httpContext.TraceIdentifier);

        (int statusCode, string? title) = MapException(exception);

        httpContext.Response.StatusCode = statusCode;

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Type = GetProblemType(statusCode),
            Instance = httpContext.Request.Path,
            Detail = GetSafeErrorMessage(exception)
        };

        problemDetails.Extensions["traceId"] = httpContext.TraceIdentifier;
        problemDetails.Extensions["timestamp"] = DateTimeOffset.UtcNow;

        return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            ProblemDetails = problemDetails
        });
    }

    private static (int StatusCode, string Title) MapException(Exception exception) => exception switch
    {
        AppException appEx => (
        (int)appEx.StatusCode,
        ReasonPhrases.GetReasonPhrase((int)appEx.StatusCode)),

        JsonException => (
            StatusCodes.Status400BadRequest,
            ReasonPhrases.GetReasonPhrase(StatusCodes.Status400BadRequest)),

        BadHttpRequestException => (
            StatusCodes.Status400BadRequest,
            ReasonPhrases.GetReasonPhrase(StatusCodes.Status400BadRequest)),

        ArgumentException => (
            StatusCodes.Status400BadRequest,
            ReasonPhrases.GetReasonPhrase(StatusCodes.Status400BadRequest)),

        UnauthorizedAccessException => (
            StatusCodes.Status401Unauthorized,
            ReasonPhrases.GetReasonPhrase(StatusCodes.Status401Unauthorized)),

        _ => (
            StatusCodes.Status500InternalServerError,
            ReasonPhrases.GetReasonPhrase(StatusCodes.Status500InternalServerError))
    };

    private static string GetProblemType(int statusCode) => statusCode switch
    {
        400 => "https://tools.ietf.org/html/rfc9110#section-15.5.1",
        401 => "https://tools.ietf.org/html/rfc9110#section-15.5.2",
        403 => "https://tools.ietf.org/html/rfc9110#section-15.5.4",
        404 => "https://tools.ietf.org/html/rfc9110#section-15.5.5",
        409 => "https://tools.ietf.org/html/rfc9110#section-15.5.10",
        _ => "https://tools.ietf.org/html/rfc9110#section-15.6.1"
    };

    private string? GetSafeErrorMessage(Exception exception)
    {
        Exception rootException = exception.InnerException ?? exception;

        if (rootException is JsonException)
        {
            return "The request body contains invalid or unexpected JSON.";
        }

        // Only expose details in development
        if (environment.IsDevelopment())
        {
            return exception.Message;
        }

        // In production, only expose messages from our own exceptions
        return exception is AppException ? exception.Message : null;
    }
}
