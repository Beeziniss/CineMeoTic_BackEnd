using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace CineMeoTic.UserService.API.Services
{
    public class JsonWebTokenService : IJsonWebTokenService
    {
        public string GenerateAccessToken(IEnumerable<Claim> claims, int expiration = 7)
        {
            string secretKey = Environment.GetEnvironmentVariable("JWT_SECRET_KEY") ?? throw new Exception("JWT_SECRET_KEY not found");

            byte[] symmetricKey = Encoding.UTF8.GetBytes(secretKey);

            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();

            SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
            {
                CompressionAlgorithm = SecurityAlgorithms.HmacSha256,
                
                Issuer = Environment.GetEnvironmentVariable("JWT_ISSUER"),
                Audience = Environment.GetEnvironmentVariable("JWT_AUDIENCE"),
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(expiration),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(symmetricKey), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];

            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
    }
}
