using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace PaymentGateway.Api.Exceptions.Filters;

public class ApiExceptionHandler(ILogger<ApiExceptionHandler> logger) : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        var exception = context.Exception;
        var statusCode = DetermineStatusCode(exception);
        var errorDetails = GetErrorDetails(exception);

        logger.LogError(exception, "Unhandled exception occurred: {ErrorDetails}", errorDetails);

        context.Result = new ObjectResult(new
        {
            error = errorDetails.Message,
            detail = errorDetails.Detail
        })
        {
            StatusCode = statusCode
        };

        context.ExceptionHandled = true;
    }

    private int DetermineStatusCode(Exception ex)
    {
        return ex switch
        {
            ValidationException => StatusCodes.Status400BadRequest,
            KeyNotFoundException => StatusCodes.Status404NotFound,
            UnauthorizedAccessException => StatusCodes.Status403Forbidden,
            OperationCanceledException => StatusCodes.Status499ClientClosedRequest,
            NotImplementedException => StatusCodes.Status501NotImplemented,
            _ => StatusCodes.Status500InternalServerError
        };
    }

    private (string Message, string Detail) GetErrorDetails(Exception ex)
    {
        return ex switch
        {
            ValidationException e => ("Validation failure", e.Message),
            KeyNotFoundException e => ("Resource not found", e.Message),
            UnauthorizedAccessException e => ("Access denied", e.Message),
            OperationCanceledException e => ("Request cancelled by the client", e.Message),
            NotImplementedException e => ("Feature not implemented", e.Message),
            _ => ("An unexpected error occurred", ex.Message)
        };
    }
}