using LoyalCode.Dmitriev.Ivan.Infrustructure.Jwt;
using LoyalCode.Dmitriev.Ivan.Infrustructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoyalCode.Dmitriev.Ivan.Infrustructure
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddMemoryCache();

            services.AddDbContext<AppDbContext>(options =>
                 options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"),
                     b => b.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName)));

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IUserFavoriteCurrencyRepository, UserFavoriteCurrencyRepository>();
            services.AddScoped<ICurrencyRepository, CurrencyRepository>();

            services.AddScoped<IPasswordService, PasswordService>();
            services.AddScoped<ITokenService, TokenService>();

            var key = Encoding.ASCII.GetBytes(configuration["Jwt:Key"] ?? string.Empty);

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = configuration["Jwt:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = configuration["Jwt:Audience"],
                    ValidateLifetime = true,
                };
            });

            return services;
        }
    }
}
