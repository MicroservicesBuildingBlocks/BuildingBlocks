using System.IdentityModel.Tokens.Jwt;
using BuildingBlocks.Core;
using BuildingBlocks.Mvc;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    public static class SetupIdentity
    {
        public static IServiceCollection AddCustomIdentity(
            this IServiceCollection services, 
            IApiInfo apiInfo
            )
        {
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

            }).AddJwtBearer(options =>
            {
                options.Authority = apiInfo.AuthenticationAuthority;
                options.RequireHttpsMetadata = false;
                options.Audience = apiInfo.JwtBearerAudience;
            });

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<IUser, HttpContextUser>();

            return services;
        }
    }
}
