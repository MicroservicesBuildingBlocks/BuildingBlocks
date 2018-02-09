using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

using Swashbuckle.AspNetCore.Examples;
using Swashbuckle.AspNetCore.Swagger;

using BuildingBlocks.Core;

namespace BuildingBlocks.Swagger
{
    public static class Startup
    {
        public static IServiceCollection AddCustomSwagger(
            this IServiceCollection services,
            IApiInfo apiInfo

        ) => services
            .AddSwaggerGen(options =>
            {
                options.DescribeAllEnumsAsStrings();

                options.SwaggerDoc(apiInfo.Version, new Info
                {
                    Title = apiInfo.Title,
                    Version = apiInfo.Version,
                    Description = apiInfo.Version
                });

                if (apiInfo.AuthenticationAuthority != null)
                {
                    options.AddSecurityDefinition("oauth2", new OAuth2Scheme
                    {
                        Type = "oauth2",
                        Flow = "implicit",
                        AuthorizationUrl = $"{apiInfo.AuthenticationAuthority}/connect/authorize",
                        TokenUrl = $"{apiInfo.AuthenticationAuthority}/connect/token",
                        Scopes = apiInfo.Scopes
                    });
                }
                
                options.OperationFilter<AuthorizeCheckOperationFilter>(apiInfo);
                options.OperationFilter<ExamplesOperationFilter>();
                options.OperationFilter<DescriptionOperationFilter>();
            });

        public static IApplicationBuilder UseCustomSwagger(
            this IApplicationBuilder app,
            IApiInfo apiInfo
        ) => app
            .UseSwagger()
            .UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint($"/swagger/{apiInfo.Version}/swagger.json", $"{apiInfo.Title} {apiInfo.Version}");
                if (apiInfo.AuthenticationAuthority != null)
                {
                    c.ConfigureOAuth2(
                        apiInfo.SwaggerAuthInfo.ClientId,
                        apiInfo.SwaggerAuthInfo.Secret,
                        apiInfo.SwaggerAuthInfo.Realm,
                        $"{apiInfo.Title} - ${apiInfo.Version} - Swagger UI"
                    );
                }
            });
    }
}
