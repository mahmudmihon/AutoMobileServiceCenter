using ASC.Models.BaseTypes;
using ASC.Models.Models;
using ASC.Utilities;
using ASC.Web.Areas.ServiceRequests.Models;
using ASC.Web.Configuration;
using ASC.Web.Controllers;
using ASC.Web.DataAccess.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ASC.Web.Areas.ServiceRequests.Controllers
{
    [Area("ServiceRequests")]
    public class DashboardController : BaseController
    {
        private IOptions<ApplicationSettings> _settings;
        private IRepository<ServiceRequest> _repo;

        public DashboardController(IOptions<ApplicationSettings> settings, IRepository<ServiceRequest> repo)
        {
            _settings = settings;
            _repo = repo;
        }

        public IActionResult Dashboard()
        {
            List<ServiceRequest> serviceRequests = new List<ServiceRequest>();
            Dictionary<string, int> activeServiceRequests = new Dictionary<string, int>();

            if (HttpContext.User.IsInRole(Roles.Admin.ToString()))
            {
                serviceRequests = (List<ServiceRequest>)_repo.GetByCondition($"RequestedDate >= \"{DateTime.Now.AddDays(-7)}\" && Status == \"New\" || Status == \"InProgress\" || Status == \"Initiated\" || Status == \"RequestForInformation\"");
                var activeRequests = _repo.GetByCondition($"Status == \"InProgress\" || Status == \"Initiated\"");
                if(activeRequests.Any())
                {
                    activeServiceRequests = activeRequests.GroupBy(x => x.ServiceEngineer).ToDictionary(p => p.Key, p => p.Count());
                }
            }
            else if (HttpContext.User.IsInRole(Roles.Engineer.ToString()))
            {
                serviceRequests = _repo.GetByCondition($"RequestedDate >= \"{DateTime.Now.AddDays(-7)}\" && ServiceEngineer == \"{HttpContext.User.GetCurrentUserDetails().Email}\" && Status == \"New\" || Status == \"InProgress\" || Status == \"Initiated\" || Status == \"RequestForInformation\"").ToList();
            }
            else
            {
                serviceRequests = _repo.GetByCondition($"RequestedDate >= \"{DateTime.Now.AddDays(-7)}\" && Email == \"{HttpContext.User.GetCurrentUserDetails().Email}\"").ToList();
            }
            return View(new DashboardViewModel {
                ServiceRequests = serviceRequests.OrderByDescending(p => p.RequestedDate).ToList(),
                ActiveServiceRequests = activeServiceRequests
            });
        }

        public IActionResult TestException()
        {
            var i = 0;
            // Should through Divide by zero error
            var j = 1 / i;
            return View();
        }
    }
}
