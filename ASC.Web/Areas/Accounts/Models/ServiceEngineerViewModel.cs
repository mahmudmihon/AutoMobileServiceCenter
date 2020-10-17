using ASC.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASC.Web.Areas.Accounts.Models
{
    public class ServiceEngineerViewModel
    {
        public List<ApplicationUser> ServiceEngineers { get; set; }
        public ServiceEngineerRegistrationViewModel Registration { get; set; }
    }
}
