using MyAccounting.Data;
using MyAccounting.Repository;

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

        public async Task InvokeAsync(HttpContext context, SqlDBContext dbContext)
        {
            if (context.Request.Path.StartsWithSegments("/loginswagger"))
            {
                string apiKey = context.Request.Query["key"].ToString();
                if (apiKey == _key)
                {
                    // ذخیره وضعیت احراز هویت در Session
                    context.Session.SetString("SwaggerAuthenticated", "true");
                    context.Response.StatusCode = 200;
                    await context.Response.WriteAsync("Success");
                    return;
                }
                else
                {
                    context.Response.StatusCode = 400;
                    await context.Response.WriteAsync("Bad Request");
                    return;
                }
            }
            else if (context.Request.Path.StartsWithSegments("/swagger"))
            {
                // بررسی اگر قبلاً احراز هویت شده
                if (context.Session.GetString("SwaggerAuthenticated") == "true")
                {
                    await _next(context);
                    return;
                }
                else
                {
                    context.Response.StatusCode = 401;
                    await context.Response.WriteAsync("Unauthorized - Invalid API Key");
                    return;
                }
            }
            else
            {
                await _next(context);
                return;
            }
        }
    }

    // Extension method برای ثبت middleware
    public static class SwaggerAuthMiddlewareExtensions
    {
        public static IApplicationBuilder UseSwaggerAuth(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<SwaggerAuthMiddleware>();
        }
    }
}
