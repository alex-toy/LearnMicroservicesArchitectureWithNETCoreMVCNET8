using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Ocelot.DependencyInjection;
using System.Text;

namespace Mango.GatewaySolution.Extensions
{
    public static class WebApplicationBuilderExtensions
    {
        public static void ConfigureAuthentication(this WebApplicationBuilder builder)
        {
            IConfigurationSection settingsSection = builder.Configuration.GetSection("ApiSettings");
            string? secret = settingsSection.GetValue<string>("Secret");
            string? issuer = settingsSection.GetValue<string>("Issuer");
            string? audience = settingsSection.GetValue<string>("Audience");

            byte[] key = Encoding.ASCII.GetBytes(secret);

            builder.Services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(x =>
            {
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = issuer,
                    ValidAudience = audience,
                    ValidateAudience = true
                };
            });
        }

        public static void ConfigureOcelot(this WebApplicationBuilder builder)
        {
            if (builder.Environment.EnvironmentName.ToString().ToLower().Equals("production"))
            {
                builder.Configuration.AddJsonFile("ocelot.Production.json", optional: false, reloadOnChange: true);
            }
            else
            {
                builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);
            }

            builder.Services.AddOcelot(builder.Configuration);
        }
    }
}
