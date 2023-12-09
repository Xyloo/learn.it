namespace learn.it.Utils
{
    public class JwtBlacklistMiddleware
    {
        private readonly RequestDelegate _next;

        public JwtBlacklistMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var token = context.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            if (InvalidJwtTokens.IsTokenInvalid(token))
            {
                context.Response.StatusCode = 401; // Unauthorized
                await context.Response.WriteAsync("Token nieprawidłowy - zaloguj się ponownie.");
            }
            else
            {
                await _next(context);
            }
        }
    }

}
