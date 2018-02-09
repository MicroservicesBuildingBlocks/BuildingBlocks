using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Polly;
using Polly.Wrap;

namespace BuindingBlocks.Resilience.Http
{
    public delegate IEnumerable<Policy> PolicyFactory(string origin);

    public class ResilientHttpClient : IHttpClient
    {
        private readonly PolicyFactory _policyFactory;
        private readonly StandardHttpClient _standardHttpClient;

        private readonly ConcurrentDictionary<string, PolicyWrap> _policyWrappers =
            new ConcurrentDictionary<string, PolicyWrap>();

        public ResilientHttpClient(
            PolicyFactory policyFactory,
            IHttpContextAccessor accessor
            )
        {
            _policyFactory = policyFactory;
            _standardHttpClient = new StandardHttpClient(accessor);

        }

        public Task<string> GetStringAsync(string uri, Authorization authorization = default(Authorization),
            CancellationToken cancellationToken = default(CancellationToken)) => HttpInvoker(
            new Uri(uri).GetOriginFromUri(),
            () => _standardHttpClient.GetStringAsync(
                uri, authorization, cancellationToken
            )
        );

        public Task<HttpResponseMessage> PutAsync<T>(string uri, T item, string requestId = null, Authorization authorization = default(Authorization),
            CancellationToken cancellationToken = default(CancellationToken)) => HttpInvoker(
            new Uri(uri).GetOriginFromUri(),
            () => _standardHttpClient.PutAsync(
                uri, item, requestId, authorization, cancellationToken
            )
        );
        
        private async Task<T> HttpInvoker<T>(string origin, Func<Task<T>> action)
        {
            var normalizedOrigin = origin?.Trim()?.ToLower();

            if (!_policyWrappers.TryGetValue(normalizedOrigin, out var policyWrap))
            {
                var policies = _policyFactory(normalizedOrigin).ToArray();
                policyWrap = Policy.WrapAsync(policies);
                _policyWrappers.TryAdd(normalizedOrigin, policyWrap);
            }

            return await policyWrap.ExecuteAsync(action, new Context(normalizedOrigin));
        }
    }

    public static class UriExtensions
    {
        public static string GetOriginFromUri(this Uri uri) =>
            $"{uri.Scheme}://{uri.DnsSafeHost}:{uri.Port}";
    }
}
