using Microsoft.AspNetCore.Mvc;

namespace MyAccounting.ViewModels
{
    public class Response
    {
        public static string SetMessage(string? message)
        {
            if (message != null)
            {
                return $" - {message}";
            }
            return string.Empty;
        }

        //public static OkResult OK()
        //{
        //    return new OkResult();
        //}

        public static async Task WriteOkJsonAsync(HttpContext context, string? message = null)
        {
            context.Response.StatusCode = StatusCodes.Status200OK;
            await context.Response.WriteAsJsonAsync(new
            {
                StatusCodes = context.Response.StatusCode,
                success = true,
                message = $"Success{SetMessage(message)}"
            });
        }

        //public static IActionResult NotFound()
        //{
        //    return new NotFoundResult();
        //}

        public static async Task WriteNotFoundJsonAsync(HttpContext context, string? message = null)
        {
            context.Response.StatusCode = StatusCodes.Status404NotFound;
            await context.Response.WriteAsJsonAsync(new
            {
                StatusCodes = context.Response.StatusCode,
                success = false,
                message = $"Not Found{SetMessage(message)}"
            });
        }

        //public static IActionResult BadRequest()
        //{
        //    return new BadRequestResult();
        //}

        public static async Task WriteBadRequestJsonAsync(HttpContext context, string? message = null)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await context.Response.WriteAsJsonAsync(new
            {
                StatusCodes = context.Response.StatusCode,
                success = false,
                message = $"Bad Request{SetMessage(message)}"
            });
        }

        //public static IActionResult Forbidden()
        //{
        //    return new ForbidResult();
        //}

        public static async Task WriteForbiddenJsonAsync(HttpContext context, string? message = null)
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            await context.Response.WriteAsJsonAsync(new
            {
                StatusCodes = context.Response.StatusCode,
                success = false,
                message = $"Forbidden{SetMessage(message)}"
            });
        }

        //public static IActionResult Unauthorized()
        //{
        //    return new UnauthorizedResult();
        //}

        public static async Task WriteUnauthorizedJsonAsync(HttpContext context, string? message = null)
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsJsonAsync(new
            {
                StatusCodes = context.Response.StatusCode,
                success = false,
                message = $"Unauthorized{SetMessage(message)}"
            });
        }

        //public static IActionResult Error()
        //{
        //    return new StatusCodeResult(500);
        //}

        public static async Task WriteErrorAsync(HttpContext context, string? message = null)
        {
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await context.Response.WriteAsJsonAsync(new
            {
                StatusCodes = context.Response.StatusCode,
                success = false,
                message = $"Internal Server Error{SetMessage(message)}"
            });
        }
    }
}
