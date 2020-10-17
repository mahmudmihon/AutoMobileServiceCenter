using ASC.Models.Models;
using System.Collections.Generic;

namespace ASC.Web.Areas.ServiceRequests.Models
{
    public class DashboardViewModel
    {
        public List<ServiceRequest> ServiceRequests { get; set; }
        public Dictionary<string, int> ActiveServiceRequests { get; set; }
    }
}
