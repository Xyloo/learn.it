using System.IdentityModel.Tokens.Jwt;

namespace learn.it.Utils
{
    public static class InvalidJwtTokens
    {
        private static readonly Timer Timer = new(ClearInvalidTokens, null, TimeSpan.FromMinutes(5),
            TimeSpan.FromMinutes(15));

        public static List<string> _invalidTokens = new();

        public static void AddToken(string token)
        {
            _invalidTokens.Add(token);
        }

        private static void ClearInvalidTokens(object? state)
        {
            _invalidTokens.RemoveAll(token => DateTime.UtcNow > new JwtSecurityToken(token).ValidTo);
        }

        public static bool IsTokenInvalid(string token)
        {
            return _invalidTokens.Contains(token);
        }
    }
}