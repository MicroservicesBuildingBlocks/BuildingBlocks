using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using BuildingBlocks.Core;
using Microsoft.AspNetCore.Http;

namespace BuildingBlocks.Mvc
{
    public class HttpContextUser : IUser
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public HttpContextUser(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string Id => _httpContextAccessor.HttpContext.User.Claims.First(c => c.Type == "sub").Value;
        public string Name => _httpContextAccessor.HttpContext.User.Identity.Name;
        public IEnumerable<Claim> Claims => _httpContextAccessor.HttpContext.User.Claims;
    }
}