using Microsoft.AspNetCore.Builder;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class SetupPermissiveCors
    {
        public static IServiceCollection AddPermissiveCors(
            this IServiceCollection services
        ) => services
            .AddCors(options =>
            {
                options.AddPolicy("PermissiveCorsPolicy", builder => builder
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials()
                );
            });

        public static IApplicationBuilder UsePermissiveCors(this IApplicationBuilder app)
            => app.UseCors("PermissiveCorsPolicy");
    }
}
