using Microsoft.AspNetCore.Identity;

namespace ASC.Web.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string IsActive { get; set; }
    }
}
