using System.IdentityModel.Tokens.Jwt;

namespace SmartAgro.Services
{
    public static class JWTService
    {
        public static Guid GetUserIdFromToken(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var readedToken = handler.ReadJwtToken(token);

            var claim = readedToken.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Sub);

            return Guid.Parse(claim.Value);
        }

        public static JwtSecurityToken ReadToken(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var readedToken = handler.ReadJwtToken(token);
            return readedToken;
        }
    }
}
