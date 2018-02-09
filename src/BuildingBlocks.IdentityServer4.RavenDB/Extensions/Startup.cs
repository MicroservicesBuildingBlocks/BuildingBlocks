using System;
using BuildingBlocks.IdentityServer4.RavenDB.Stores;
using IdentityServer4.Stores;
using Raven.Client.Documents;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    // ReSharper disable once InconsistentNaming
    public static class Startup
    {
        public static IIdentityServerBuilder AddRavenDBConfigurationStore(
            this IIdentityServerBuilder builder,
            IDocumentStore documentStore, string databaseName,
            Action<IRavenDBClientStore, IRavenDBResourceStore> setup
        )
        {
            var session = documentStore.OpenAsyncSession(databaseName);

            builder.Services
                .AddTransient<IClientStore>((sp) => new ClientStore(session))
                .AddTransient<IResourceStore>((sp) => new ResourceStore(session));

            setup(new ClientStore(session), new ResourceStore(session));

            return builder;
        }

        public static IIdentityServerBuilder AddRavenDBOperationalStore(
            this IIdentityServerBuilder builder,
            IDocumentStore documentStore, string databaseName
        )
        {
            var session = documentStore.OpenAsyncSession(databaseName);

            builder.Services
                .AddScoped<IPersistedGrantStore>(sp => new PersistedGrantStore(session));

            return builder;
        }
    }
}
