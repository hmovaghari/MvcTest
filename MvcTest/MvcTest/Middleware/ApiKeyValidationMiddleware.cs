using MyAccounting.Data;
using MyAccounting.Repository;
using MyAccounting.ViewModels;

namespace MyAccounting.Middleware
{
    public class ApiKeyValidationMiddleware
    {
        private readonly RequestDelegate _next;

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
            if (!context.Request.Query.TryGetValue("apiKey", out var apiKeyValues))
            {
                await Response.WriteUnauthorizedJsonAsync(context, "API Key is required");
                return;
            }

            var apiKey = apiKeyValues.FirstOrDefault();
            if (string.IsNullOrWhiteSpace(apiKey))
            {
                await Response.WriteUnauthorizedJsonAsync(context, "API Key cannot be empty");
                return;
            }

            // اعتبارسنجی API Key
            var apiKeyRepository = new ApiKeyRepository(dbContext);
            var isValid = await apiKeyRepository.IsValidApiKeyAsync(apiKey);

            if (!isValid)
            {
                await Response.WriteForbiddenJsonAsync(context, "Invalid or inactive API Key");
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
