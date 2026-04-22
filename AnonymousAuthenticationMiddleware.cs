using System.Security.Claims;

namespace EduTests;

public class AnonymousAuthenticationMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        if (!context.User.Identity.IsAuthenticated)
        {
            var cookieName = "AnonymousId";
            if (!context.Request.Cookies.TryGetValue(cookieName, out var anonymousId))
            {
                anonymousId = Guid.NewGuid().ToString();
                context.Response.Cookies.Append(cookieName, anonymousId, new CookieOptions()
                {
                    HttpOnly = true,
                    SameSite = SameSiteMode.Lax,
                    Expires = DateTimeOffset.UtcNow.AddYears(1)
                });
            }

            context.Items["AnonymousId"] = anonymousId;
        }
        await next(context);
    }
}