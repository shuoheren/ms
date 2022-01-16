using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using ApplicationCore.ServiceInterfaces;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string Name => GetName();

        public int? UserId => GetUserId();
        public bool IsAuthenticated => GetAuthenticated();
        public string UserName => _httpContextAccessor.HttpContext?.User.Identity?.Name;

        public string FullName => _httpContextAccessor.HttpContext?.User.Claims
            .FirstOrDefault(c => c.Type == ClaimTypes.GivenName)
            ?.Value + " " + _httpContextAccessor.HttpContext?.User.Claims
            .FirstOrDefault(c =>
                c.Type ==
                ClaimTypes
                    .Surname)
            ?.Value;

        public string Email => _httpContextAccessor.HttpContext?.User.Claims
            .FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

        public string RemoteIpAddress => GetRemoteAddress();

        public IEnumerable<Claim> GetClaimsIdentity()
        {
            return _httpContextAccessor.HttpContext?.User.Claims;
        }

        public IEnumerable<string> Roles => GetRoles();
        public string ProfilePictureUrl { get; set; }
        public bool IsAdmin => GetIsAdmin();

        public bool IsSuperAdmin => GetIsSuperAdmin();

        private string GetRemoteAddress()
        {
            return _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString();
        }

        private bool GetIsAdmin()
        {
            var roles = Roles;
            return roles.Any(r => r.Contains("Admin"));
        }

        private bool GetIsSuperAdmin()
        {
            var roles = Roles;
            return roles.Any(r => r.Contains("SuperAdmin"));
        }

        private int? GetUserId()
        {
            var userId =
                Convert.ToInt32(_httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            return userId;
        }

        private bool GetAuthenticated()
        {
            return _httpContextAccessor.HttpContext?.User.Identity != null &&
                   _httpContextAccessor.HttpContext != null &&
                   _httpContextAccessor.HttpContext.User.Identity.IsAuthenticated;
        }

        private string GetName()
        {
            return _httpContextAccessor.HttpContext.User.Identity.Name ??
                   _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
        }

        private IEnumerable<string> GetRoles()
        {
            var claims = GetClaimsIdentity();
            var roles = new List<string>();
            foreach (var claim in claims)
                if (claim.Type == ClaimTypes.Role)
                    roles.Add(claim.Value);
            return roles;
        }
    }
}