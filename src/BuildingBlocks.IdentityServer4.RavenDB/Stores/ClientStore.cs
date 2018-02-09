using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using Raven.Client.Documents;
using Raven.Client.Documents.Session;

namespace BuildingBlocks.IdentityServer4.RavenDB.Stores
{
    public interface IRavenDBClientStore : IClientStore
    {
        Task StoreAsync(Client client);
        Task<bool> HasStoredClients();
    }
    internal class ClientStore : IRavenDBClientStore
    {
        private readonly IAsyncDocumentSession _session;

        public ClientStore(IAsyncDocumentSession session)
        {
            _session = session;
        }

        public async Task<Client> FindClientByIdAsync(string clientId) =>
            (await _session.LoadAsync<Client>(RavenClient.DocId(clientId)));

        public async Task StoreAsync(Client client)
        {
            await _session.StoreAsync(client, RavenClient.DocId(client.ClientId));
            await _session.SaveChangesAsync();
        }

        public Task<bool> HasStoredClients()
        {
            return _session.Query<Client>()
                .Customize(o => o.WaitForNonStaleResults())
                .AnyAsync();
        }

        internal class RavenClient : Client
        {
            public string Id => DocId(ClientId);

            public static string DocId(string clientId)
            {
                return $"client/{clientId}";
            }
        }
    }
    
}
