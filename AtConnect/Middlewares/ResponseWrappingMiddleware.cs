using AtConnect.DTOs;
using System.Text.Json;

namespace AtConnect.Middlewares
{
    public class ResponseWrappingMiddleware
    {
        private readonly RequestDelegate _next;

        public ResponseWrappingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var originalBody = context.Response.Body;
            using var memStream = new MemoryStream();
            context.Response.Body = memStream;

            await _next(context);

            // لو Error → سيبه لـ ErrorMiddleware
            if (context.Response.StatusCode >= 400)
            {
                memStream.Seek(0, SeekOrigin.Begin);
                await memStream.CopyToAsync(originalBody);
                context.Response.Body = originalBody;
                return;
            }

            // قراءة الـ Body الأصلي
            memStream.Seek(0, SeekOrigin.Begin);
            var raw = await new StreamReader(memStream).ReadToEndAsync();
            context.Response.Body = originalBody;

            object data = null!;
            if (!string.IsNullOrWhiteSpace(raw))
            {
                try { data = JsonSerializer.Deserialize<object>(raw)!; }
                catch { data = raw; }
            }

            // الرسالة: يا إمّا Controller يحددها، أو Default
            var msg = context.Items.ContainsKey("response_message")
                ? context.Items["response_message"]?.ToString()
                : "Request completed successfully";

            var wrapped = new ApiResponse<object>(true, msg!, data);

            var json = JsonSerializer.Serialize(wrapped);

            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(json);
        }
    }

}
