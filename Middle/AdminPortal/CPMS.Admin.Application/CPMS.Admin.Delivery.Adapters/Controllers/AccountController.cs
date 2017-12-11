using System.Web.Mvc;
using System.Web.Security;
using CPMS.Admin.Presentation;

namespace CPMS.Admin.Delivery.Adapters.Controllers
{
    public class AccountController : Controller
    {
        private readonly AdminPresentationService _adminPresentationService;

        public AccountController(AdminPresentationService adminPresentationService)
        {
            _adminPresentationService = adminPresentationService;
        }

        public ActionResult Login(string returnUrl)
        {
            return View(new LoginViewModel {ReturnUrl = returnUrl});
        }

        [HttpPost]
        public ActionResult Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var userValid = _adminPresentationService.IsUserValid(model);

                if (userValid)
                {
                    FormsAuthentication.SetAuthCookie(model.Username, model.RememberMe);
                    var returnUrl = model.ReturnUrl;

                    if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/")
                        && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
                    {
                        return Redirect(returnUrl);
                    }

                    return Redirect(Url.Content("~/"));
                }

                ModelState.AddModelError("", "The user name or password provided is incorrect.");
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult LogOut()
        {
            FormsAuthentication.SignOut();
            return Redirect(Url.Content("~/"));
        }

    }
}