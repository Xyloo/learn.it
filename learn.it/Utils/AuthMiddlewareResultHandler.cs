using learn.it.Exceptions;
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
                throw new UnauthorizedAccessException("Unauthorized - please log in.");
            }

            if (authorizeResult.Forbidden)
            {
                throw new ForbiddenAccessException("Forbidden - you do not have access to this endpoint.");

            }
            
            await defaultHandler.HandleAsync(next, context, policy, authorizeResult);
        }
    }
}
