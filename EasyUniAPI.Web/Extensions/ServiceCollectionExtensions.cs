using EasyUniAPI.Common.Configurations;
using EasyUniAPI.Core.Implementations;
using EasyUniAPI.Core.Interfaces;
using EasyUniAPI.DataAccess;
using EasyUniAPI.DataAccess.Entities;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace EasyUniAPI.Web.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void RegisterAuth(this IServiceCollection services, IConfiguration configuration)
        {
            var jwtOptsSection = configuration.GetSection("Jwt");
            services.Configure<JwtOptions>(jwtOptsSection);

            var jwtOptions = jwtOptsSection.Get<JwtOptions>();
            ArgumentNullException.ThrowIfNull(jwtOptions);

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtOptions.Issuer,
                    ValidAudience = jwtOptions.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Key))
                };
            });

            services.AddAuthorization();
        }

        public static void RegisterCors(this IServiceCollection services, IConfiguration configuration, string corsPolicyName)
        {
            var allowedOrigins = configuration
                .GetSection("Cors:AllowedOrigins").Get<string[]>();
            ArgumentNullException.ThrowIfNull(allowedOrigins);

            services.AddCors(options =>
            {
                options.AddPolicy(corsPolicyName, policy =>
                {
                    policy.WithOrigins(allowedOrigins)
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials();
                });
            });
        }

        public static void RegisterServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<EasyUniDbContext>(x => x.UseSqlServer(configuration.GetConnectionString("EasyUni")));

            services.AddValidatorsFromAssemblies(AppDomain.CurrentDomain.GetAssemblies());

            services.AddHttpContextAccessor();

            services.AddScoped<IRepository<User, string>, Repository<User, string>>();
            services.AddScoped<IRepository<UserRole, long>, Repository<UserRole, long>>();
            services.AddScoped<IRepository<Role, int>, Repository<Role, int>>();

            services.AddScoped<IClaimsProvider, ClaimsProvider>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IPasswordHashService, PasswordHashService>();
        }
    }
}
