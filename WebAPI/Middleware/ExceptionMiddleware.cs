using System.Net;
using System.Text.Json;

namespace WebAPI.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Произошла необработанная ошибка");

                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            context.Response.ContentType = "application/json";

            var response = new
            {
                message = ex.Message,
                statusCode = (int)HttpStatusCode.InternalServerError
            };

            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            // Обработка конкретных типов ошибок
            if (ex is KeyNotFoundException)
            {
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                response = new
                {
                    message = ex.Message,
                    statusCode = (int)HttpStatusCode.NotFound
                };
            }
            else if (ex is UnauthorizedAccessException)
            {
                context.Response.StatusCode = 401;
            }
            else if (ex is ArgumentException)
            {
                context.Response.StatusCode = 400;
            }

            return context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}
