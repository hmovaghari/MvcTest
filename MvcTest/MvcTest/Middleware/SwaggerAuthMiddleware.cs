namespace MyAccounting.Middleware
{
    public class SwaggerAuthMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly string _key;

        public SwaggerAuthMiddleware(RequestDelegate next, IConfiguration configuration)
        {
            _next = next;
            _key = configuration["SwaggerKey"];
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.Path.StartsWithSegments("/swagger"))
            {
                // بررسی اگر قبلاً احراز هویت شده
                if (context.Session.GetString("SwaggerAuthenticated") == "true")
                {
                    await _next(context);
                    return;
                }

                // بررسی API Key در query string یا header
                string apiKey = context.Request.Query["key"].ToString();
                if (string.IsNullOrEmpty(apiKey))
                {
                    apiKey = context.Request.Headers["X-Api-Key"].ToString();
                }

                if (_key == null || apiKey != _key)
                {
                    context.Response.StatusCode = 401;
                    await context.Response.WriteAsync("Unauthorized - Invalid API Key");
                    return;
                }

                // ذخیره وضعیت احراز هویت در Session
                context.Session.SetString("SwaggerAuthenticated", "true");
            }

            await _next(context);
        }
    }

    // Extension method برای ثبت middleware
    public static class SwaggerAuthMiddlewareExtensions
    {
        public static IApplicationBuilder UseSwaggerAuth(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ApiKeyValidationMiddleware>();
        }
    }
}