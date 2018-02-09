using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using BuildingBlocks.AspnetCoreIdentity.RavenDB;
using Raven.Client.Documents;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    public static class Startup
    {
        public static IServiceCollection AddRavenDBIdentity(
            this IServiceCollection services,
            IDocumentStore documentStore, string databaseName
            )
        {
            var session = documentStore.OpenAsyncSession(databaseName);
            services.AddIdentity<User, Role>();

            services.AddScoped<IUserStore<User>>(
                (sp) => new UserStore(session)
                );
            services.AddScoped<IRoleStore<Role>>(
                (sp) => new RoleStore(session)
                );

            return services;
        }

        public static IApplicationBuilder UseRavenDBIdentity(this IApplicationBuilder app) => app;
    }
}
