using ASC.Web.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ASC.Web.Controllers
{
    [Authorize]
    //[UserActivityFilter]
    public class BaseController : Controller
    {
    }
}
