using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.Text;
using Workspace.Infrastructure;

namespace Workspace.API
{
    public static class JwtExtensions
    {
        public static void AddJWTConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(o =>
            {
                JwtOption options = new ();
                configuration.GetSection(nameof(JwtOption)).Bind(options);

                var key = Encoding.UTF8.GetBytes(options.SecretKey);
                o.SaveToken = true;

                o.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false, // true in production
                    ValidateAudience = false, // true in production
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = options.Issuer,
                    ValidAudience = options.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ClockSkew = TimeSpan.Zero,
                };

                o.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                        {
                            context.Response.Headers.Append("IS-TOKEN-EXPIRED", "true");
                            context.Response.StatusCode = 401;
                            context.Response.ContentType = "application/json";
                        }

                        return Task.CompletedTask;
                    },
                };
            });

            services.AddAuthorization();
        }
    }
}
