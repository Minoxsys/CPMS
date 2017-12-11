using System.Web.Mvc;

namespace CPMS.Admin.Delivery.Adapters.Controllers
{
    public class ErrorController : Controller
    {
        public ActionResult AccessDenied()
        {
            return View();
        }
    }
}