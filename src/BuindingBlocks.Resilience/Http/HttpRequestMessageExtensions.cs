using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Http;

namespace BuindingBlocks.Resilience.Http
{
    public static class HttpRequestMessageExtensions
    {
        public static HttpRequestMessage CopyAuthorizationHeaderFrom(
            this HttpRequestMessage request,
            HttpContext context
        )
        {
            var authorizationHeader = context
                .Request
                .Headers["Authorization"];

            if (!string.IsNullOrEmpty(authorizationHeader))
            {
                request.Headers.Add("Authorization", new string[] { authorizationHeader });
            }

            return request;
        }

        public static HttpRequestMessage Apply(
            this HttpRequestMessage request,
            Authorization authorization
            )
        {
            if (!authorization.IsEmpty)
            {
                request.Headers.Authorization = new AuthenticationHeaderValue(
                    authorization.Method, 
                    authorization.Token
                    );
            }
            return request;
        }

    }
}