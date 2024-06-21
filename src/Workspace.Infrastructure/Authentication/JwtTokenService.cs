using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Workspace.Application;

namespace Workspace.Infrastructure
{
    public class JwtTokenService : IJwtTokenService
    {
        private readonly JwtOption jwtOptions = new();
        private readonly IHttpContextAccessor _httpContextAccessor;

        public JwtTokenService(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            configuration.GetSection(nameof(JwtOption)).Bind(jwtOptions);
            _httpContextAccessor = httpContextAccessor;
        }

        public string GenerateAccessToken(IEnumerable<Claim> claims)
        {
            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SecretKey));
            var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

            var tokenOptions = new JwtSecurityToken(
                issuer: jwtOptions.Issuer,
                audience: jwtOptions.Audience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(jwtOptions.ExpireMin),
                signingCredentials: signinCredentials
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(tokenOptions);

            return tokenString;
        }

        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            var rng = RandomNumberGenerator.Create();

            rng.GetBytes(randomNumber);

            return Convert.ToBase64String(randomNumber);
        }

        public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var key = Encoding.UTF8.GetBytes(jwtOptions.SecretKey);

            var tokenValidationParameters = new TokenValidationParameters()
            {
                ValidateIssuer = false, // true in production
                ValidateAudience = false, // true in production
                ValidateLifetime = false, // don't care about the token's expiration date
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ClockSkew = TimeSpan.Zero,
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);

            JwtSecurityToken jwtSecurityToken = securityToken as JwtSecurityToken;

            if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCulture))
            {
                throw new SecurityTokenException("Invalid Token");
            }

            return principal;
        }

        public void SetRefreshTokenCookie(string refreshToken)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true, // Không cho phép truy cập bằng JavaScript
                Secure = true, // Yêu cầu kết nối bảo mật (HTTPS)
                SameSite = SameSiteMode.None, // Cho phép gửi cookie qua các trang từ các trang không cùng nguồn gốc
                Expires = DateTime.UtcNow.AddMinutes(jwtOptions.ExpireMin) // Thiết lập thời gian sống cho cookie
            };

            _httpContextAccessor.HttpContext.Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);
        }
    }
}
