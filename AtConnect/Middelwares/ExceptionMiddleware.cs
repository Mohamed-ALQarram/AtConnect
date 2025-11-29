namespace AtConnect.Middleware
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
                _logger.LogError(ex, "Unhandled Exception");

                context.Response.ContentType = "application/json";

                var response = ex switch
                {
                    InvalidDataException => new { status = 400, message = ex.Message },
                    UnauthorizedAccessException => new { status = 403, message = ex.Message },
                    _ => new { status = 500, message = "Internal Server Error" }
                };

                context.Response.StatusCode = response.status;
                await context.Response.WriteAsJsonAsync(response);
            }
        }
    }

}
