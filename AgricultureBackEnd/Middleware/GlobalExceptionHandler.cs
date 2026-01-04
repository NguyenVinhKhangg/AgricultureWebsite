using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AgricultureBackEnd.Middleware
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;
        
        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
        {
            _logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync(Exception exception, HttpContext context, CancellationToken cancellationToken)
        {
            _logger.LogError(exception, "An unhandled exception occurred.");

            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Response.ContentType = "application/json";

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

            await HttpContext.Response.WriteAsJsonAsync(response, cancellationToken);
            return true;
        }
    }
}