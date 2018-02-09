using System;
using System.Net;
using Microsoft.AspNetCore.Http;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace BuindingBlocks.Resilience.Http
{
    public class StandardHttpClient : IHttpClient, IDisposable
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly HttpClient _httpClient;

        public StandardHttpClient(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _httpClient = new HttpClient();
        }

        public async Task<string> GetStringAsync(
            string uri, 
            Authorization authorization = default(Authorization),
            CancellationToken cancellationToken = default(CancellationToken)
            )
        {
            var message = new HttpRequestMessage(HttpMethod.Get, uri)
                .CopyAuthorizationHeaderFrom(_httpContextAccessor.HttpContext)
                .Apply(authorization);

            var response = await _httpClient.SendAsync(message, cancellationToken);

            if (response.StatusCode == HttpStatusCode.InternalServerError)
            {
                throw new HttpRequestException();
            }

            return await response.Content.ReadAsStringAsync();
        }

        public async Task<HttpResponseMessage> PutAsync<T>(
            string uri, 
            T item, string requestId = null, 
            Authorization authorization = default(Authorization),
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Put, uri)
                .CopyAuthorizationHeaderFrom(_httpContextAccessor.HttpContext)
                .Apply(authorization);
            
            requestMessage.Content = new StringContent(
                JsonConvert.SerializeObject(item), 
                System.Text.Encoding.UTF8, "application/json"
                );
            
            if (requestId != null)
            {
                requestMessage.Headers.Add("x-requestid", requestId);
            }

            var response = await _httpClient.SendAsync(requestMessage, cancellationToken);

            if (response.StatusCode == HttpStatusCode.InternalServerError)
            {
                throw new HttpRequestException();
            }

            return response;
        }

        public void Dispose()
        {
            _httpClient?.Dispose();
        }
    }
}
