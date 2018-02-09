using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using Raven.Client.Documents;
using Raven.Client.Documents.Linq;
using Raven.Client.Documents.Session;

namespace BuildingBlocks.IdentityServer4.RavenDB.Stores
{
    public interface IRavenDBResourceStore : IResourceStore
    {
        Task StoreAsync(IdentityResource identity);
        Task StoreAsync(ApiResource api);
        Task<bool> HasStoredIdentities();
        Task<bool> HasStoredApis();
    }

    internal class ResourceStore : IRavenDBResourceStore
    {
        private readonly IAsyncDocumentSession _session;

        public ResourceStore(IAsyncDocumentSession session)
        {
            _session = session;
        }

        public async Task<IEnumerable<IdentityResource>> FindIdentityResourcesByScopeAsync(IEnumerable<string> scopeNames)
        {
            var scopes = scopeNames.ToArray();

            var identities = await _session.Query<IdentityResource>()
                .Where(r => r.Name.In(scopes))
                .ToListAsync();

            return identities; //.Select(identity => identity.ToModel());
        }

        public async Task<IEnumerable<ApiResource>> FindApiResourcesByScopeAsync(IEnumerable<string> scopeNames)
        {
            var scopes = scopeNames.ToArray();

            var apis = await _session.Query<ApiResource>()
                .Where(r => r.Name.In(scopes))
                .ToListAsync();

            return apis;
        }

        public async Task<ApiResource> FindApiResourceAsync(string name)
        {
            var api = await _session.Query<ApiResource>()
                .Where(r => r.Name == name)
                .FirstOrDefaultAsync();

            return api;
        }

        public async Task<Resources> GetAllResourcesAsync()
        {
            var identities = await _session.Query<IdentityResource>()
                .ToListAsync();

            var apis = await _session.Query<ApiResource>()
                .ToListAsync();

            var result = new Resources(
                identities, //.Select(identity => identity.ToModel()), 
                apis //.Select(api => api.ToModel())
            );

            return result;
        }

        public async Task StoreAsync(IdentityResource identity)
        {
            await _session.StoreAsync(identity);
            await _session.SaveChangesAsync();
        }

        public async Task StoreAsync(ApiResource api)
        {
            await _session.StoreAsync(api);
            await _session.SaveChangesAsync();
        }

        public Task<bool> HasStoredIdentities()
        {
            return _session.Query<IdentityResource>()
                .Customize(o => o.WaitForNonStaleResults())
                .AnyAsync();
        }

        public Task<bool> HasStoredApis()
        {
            return _session.Query<ApiResource>()
                .Customize(o => o.WaitForNonStaleResults())
                .AnyAsync();
        }
    }
}
