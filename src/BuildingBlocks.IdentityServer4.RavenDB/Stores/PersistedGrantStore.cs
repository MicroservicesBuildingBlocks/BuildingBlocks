using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4.Stores;

using Raven.Client.Documents;
using Raven.Client.Documents.Linq;
using Raven.Client.Documents.Session;

namespace BuildingBlocks.IdentityServer4.RavenDB.Stores
{
    internal class PersistedGrantStore : IPersistedGrantStore
    {
        private readonly IAsyncDocumentSession _session;

        public PersistedGrantStore(IAsyncDocumentSession session)
        {
            _session = session;
        }

        public async Task StoreAsync(PersistedGrant grant)
        {
            await _session.StoreAsync(grant, RavenPersistedGrant.DocId(grant.Key));
            await _session.SaveChangesAsync();
        }

        public async Task<PersistedGrant> GetAsync(string key) =>
            (await _session.LoadAsync<PersistedGrant>(RavenPersistedGrant.DocId(key)));

        public async Task<IEnumerable<PersistedGrant>> GetAllAsync(string subjectId)
        {
            var grants = await _session
                .Query<PersistedGrant>()
                .Customize(opt => opt.WaitForNonStaleResults())
                .Where(g => g.SubjectId == subjectId)
                .ToListAsync();

            return grants;
        }


        public async Task RemoveAsync(string key)
        {
            _session.Delete(RavenPersistedGrant.DocId(key));
            await _session.SaveChangesAsync();
        }

        public async Task RemoveAllAsync(string subjectId, string clientId)
        {
            await RemoveWhereAsync(g => (g.SubjectId == subjectId) && (g.ClientId == clientId));
        }

        public async Task RemoveAllAsync(string subjectId, string clientId, string type)
        {
            await RemoveWhereAsync(g => (g.SubjectId == subjectId) && (g.ClientId == clientId) && (g.Type == type));
        }

        private async Task RemoveWhereAsync(Func<PersistedGrant, bool> @where)
        {
            var itemsToDelete =
                await _session
                    .Query<PersistedGrant>()
                    .Customize(opt => opt.WaitForNonStaleResults())
                    .Where(g => @where.Invoke(g))
                    .ToListAsync();

            foreach (var grant in itemsToDelete)
                _session.Delete(grant);

            await _session.SaveChangesAsync();
        }

        internal class RavenPersistedGrant : PersistedGrant
        {
            public string Id => DocId(Key);

            public static string DocId(string key)
            {
                return $"scope/{key}";
            }
        }
    }
}
