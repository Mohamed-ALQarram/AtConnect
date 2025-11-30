using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace AtConnect.Middleware
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlingMiddleware> _logger;

        public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);

                var problem = new ProblemDetails
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Title = "Internal Server Error",
                    Detail = ex.Message,
                    Instance = context.Request.Path
                };

                context.Response.StatusCode = problem.Status.Value;
                context.Response.ContentType = "application/problem+json";

                var json = JsonSerializer.Serialize(problem);
                await context.Response.WriteAsync(json);
            }

            // Validation errors (ModelState)
            if (!context.Response.HasStarted &&
                context.Response.StatusCode >= 400 &&
                context.Response.StatusCode < 500 &&
                context.Features.Get<IHttpResponseFeature>()?.ReasonPhrase == "Validation Failed")
            {
                var validationProblem = new ValidationProblemDetails
                {
                    Status = context.Response.StatusCode,
                    Title = "Validation Error",
                    Detail = "One or more validation errors occurred."
                };

                context.Response.ContentType = "application/problem+json";
                await context.Response.WriteAsync(JsonSerializer.Serialize(validationProblem));
            }
        }
    }

}
