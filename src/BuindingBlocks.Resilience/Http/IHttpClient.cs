using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace BuindingBlocks.Resilience.Http
{
    public interface IHttpClient
    {
        Task<string> GetStringAsync(
            string uri,
            Authorization authorization = default(Authorization),
            CancellationToken cancellationToken = default(CancellationToken)
        );

        Task<HttpResponseMessage> PutAsync<T>(
            string uri, T item,
            string requestId = null,
            Authorization authorization = default(Authorization),
            CancellationToken cancellationToken = default(CancellationToken)
            );
    }
}
