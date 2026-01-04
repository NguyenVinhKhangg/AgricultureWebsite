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
            _logger.LogError(exception, "An unhandled exception occurred.");

            var (statusCode, message) = exception switch
            {
                NotFoundException => (HttpStatusCode.NotFound, exception.Message),
                BadRequestException => (HttpStatusCode.BadRequest, exception.Message),
                DuplicateException => (HttpStatusCode.Conflict, exception.Message),
                _ => (HttpStatusCode.InternalServerError, "An unexpected error occurred.")
            };
    
            context.Response.StatusCode = (int)statusCode;
            context.Response.ContentType = "application/json";

            var response = new
            {
                success = false,
                message = message,
                statusCode = (int)statusCode,
                timestamp = DateTime.UtcNow
            };

            await context.Response.WriteAsJsonAsync(response, cancellationToken);
            return true;
        }
    }
}