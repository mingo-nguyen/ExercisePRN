// Create a new file: SP25_PRN222.WebApp/Middleware/SessionValidationMiddleware.cs
using FUDataAccess.Services;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace SP25_PRN222.WebApp.Middleware
{
    public class SessionValidationMiddleware
    {
        private readonly RequestDelegate _next;

        public SessionValidationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, AuthService authService)
        {
            // Get session ID from the current session
            var sessionId = context.Session.GetString("sessionId");

            // If there's a session ID but it's invalid in the database, log the user out
            if (!string.IsNullOrEmpty(sessionId))
            {
                bool isValid = await authService.ValidateSessionAsync(sessionId);
                if (!isValid)
                {
                    // Clear session data
                    context.Session.Clear();

                    // Redirect to login if not already there
                    if (!context.Request.Path.StartsWithSegments("/Auth/Login"))
                    {
                        context.Response.Redirect("/Auth/Login");
                        return;
                    }
                }
            }

            await _next(context);
        }
    }

    // Extension method to make it easier to add the middleware
    public static class SessionValidationMiddlewareExtensions
    {
        public static IApplicationBuilder UseSessionValidation(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<SessionValidationMiddleware>();
        }
    }
}

