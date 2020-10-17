using System;
using System.Linq;
using System.Security.Claims;

namespace ASC.Utilities
{
    public class CurrentUser
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public bool IsActive { get; set; }
        public string[] Roles { get; set; }
    }

    public static class ClaimsPrincipalExtensions
    {
        public static CurrentUser GetCurrentUserDetails(this ClaimsPrincipal principal)
        {
            return new CurrentUser
            {
                Name = principal.Claims.Where(c => c.Type == ClaimTypes.Name).Select(c => c.Value).FirstOrDefault(),
                Email = principal.Claims.Where(c => c.Type == ClaimTypes.Email).Select(c => c.Value).FirstOrDefault(),
                Roles = principal.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToArray(),
                //IsActive = Boolean.Parse(principal.Claims.Where(c => c.Type == "IsActive").Select(c => c.Value).FirstOrDefault()),
            };
        }
    }
}
