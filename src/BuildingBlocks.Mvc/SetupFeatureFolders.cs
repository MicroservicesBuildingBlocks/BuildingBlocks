// ReSharper disable once CheckNamespace

using BuildingBlocks.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class SetupFeatureFolders
    {
        public static IServiceCollection AddFeatureFoldersSupport(
            this IServiceCollection services
        )
        {
            services.Configure<RazorViewEngineOptions>(options =>
            {
                options.ViewLocationExpanders.Add(new FeaturesLocationExpander());
            });
            return services;
        }
    }
}
