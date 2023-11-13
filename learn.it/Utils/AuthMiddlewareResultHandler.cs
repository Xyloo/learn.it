using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;

namespace learn.it.Utils
{
    public class AuthMiddlewareResultHandler : IAuthorizationMiddlewareResultHandler
    {
        private readonly AuthorizationMiddlewareResultHandler defaultHandler = new();

        public async Task HandleAsync(
            RequestDelegate next,
            HttpContext context,
            AuthorizationPolicy policy,
            PolicyAuthorizationResult authorizeResult)
        {
            if (authorizeResult.Challenged)
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Unauthorized - please log in.");
            }
            else if (authorizeResult.Forbidden)
            {
                context.Response.StatusCode = 403;
                await context.Response.WriteAsync("Forbidden - you do not have access to this endpoint.");
            }
            else
            {
                await defaultHandler.HandleAsync(next, context, policy, authorizeResult);
            }
        }
    }
}
