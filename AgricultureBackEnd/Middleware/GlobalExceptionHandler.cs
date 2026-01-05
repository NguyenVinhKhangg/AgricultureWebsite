using System.Net;
using AgricultureStore.Application.Exceptions;
using Microsoft.AspNetCore.Diagnostics;

namespace AgricultureBackEnd.Middleware
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;

        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
        {
            _logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken cancellationToken)
        {
            var correlationId = context.Items["CorrelationId"]?.ToString() ?? "N/A";
            
            var (statusCode, message, logLevel) = exception switch
            {
                NotFoundException => (HttpStatusCode.NotFound, exception.Message, LogLevel.Warning),
                BadRequestException => (HttpStatusCode.BadRequest, exception.Message, LogLevel.Warning),
                DuplicateException => (HttpStatusCode.Conflict, exception.Message, LogLevel.Warning),
                InvalidOperationException => (HttpStatusCode.BadRequest, exception.Message, LogLevel.Warning),
                UnauthorizedAccessException => (HttpStatusCode.Unauthorized, "Unauthorized access", LogLevel.Warning),
                _ => (HttpStatusCode.InternalServerError, "An unexpected error occurred.", LogLevel.Error)
            };

            // Log with appropriate level and structured data
            _logger.Log(logLevel, exception,
                "Exception occurred. CorrelationId: {CorrelationId}, Type: {ExceptionType}, Path: {Path}, Method: {Method}, StatusCode: {StatusCode}",
                correlationId,
                exception.GetType().Name,
                context.Request.Path,
                context.Request.Method,
                (int)statusCode);

            context.Response.StatusCode = (int)statusCode;
            context.Response.ContentType = "application/json";

            var response = new
            {
                success = false,
                message = message,
                statusCode = (int)statusCode,
                correlationId = correlationId,
                timestamp = DateTime.UtcNow
            };

            await context.Response.WriteAsJsonAsync(response, cancellationToken);
            return true;
        }
    }
}