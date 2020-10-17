using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASC.Web.Areas.Accounts.Models
{
    public class CustomerRegistrationViewModel
    {
        public string Email { get; set; }
        public bool IsActive { get; set; }
    }
}
