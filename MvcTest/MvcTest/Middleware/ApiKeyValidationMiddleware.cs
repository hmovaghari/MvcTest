using MyAccounting.Data;
using MyAccounting.Repository;

namespace MyAccounting.Middleware
{
    public class ApiKeyValidationMiddleware
    {
        private readonly RequestDelegate _next;
        private const string API_KEY_QUERY_PARAM = "apiKey";

        public ApiKeyValidationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, SqlDBContext dbContext)
        {
            // فقط برای مسیرهای API چک شود
            if (!context.Request.Path.StartsWithSegments("/api"))
            {
                await _next(context);
                return;
            }

            // استخراج apiKey از query string
            if (!context.Request.Query.TryGetValue(API_KEY_QUERY_PARAM, out var apiKeyValues))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsJsonAsync(new
                {
                    success = false,
                    message = "API Key is required. Please provide 'apiKey' in query string."
                });
                return;
            }

            var apiKey = apiKeyValues.FirstOrDefault();
            if (string.IsNullOrWhiteSpace(apiKey))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsJsonAsync(new
                {
                    success = false,
                    message = "API Key cannot be empty."
                });
                return;
            }

            // اعتبارسنجی API Key
            var apiKeyRepository = new ApiKeyRepository(dbContext);
            var isValid = await apiKeyRepository.IsValidApiKeyAsync(apiKey);

            if (!isValid)
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsJsonAsync(new
                {
                    success = false,
                    message = "Invalid or inactive API Key."
                });
                return;
            }

            // API Key معتبر است، درخواست را ادامه دهید
            await _next(context);
        }
    }

    // Extension method برای ثبت middleware
    public static class ApiKeyValidationMiddlewareExtensions
    {
        public static IApplicationBuilder UseApiKeyValidation(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ApiKeyValidationMiddleware>();
        }
    }
}
