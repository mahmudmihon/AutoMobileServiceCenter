using ASC.Web.Configuration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace ASC.Web.Controllers
{
    public class DashboardController : BaseController
    {
        private IOptions<ApplicationSettings> _settings;

        public DashboardController(IOptions<ApplicationSettings> settings)
        {
            _settings = settings;
        }

        [AllowAnonymous]
        public IActionResult Dashboard()
        {
            return View();
        }
    }
}
