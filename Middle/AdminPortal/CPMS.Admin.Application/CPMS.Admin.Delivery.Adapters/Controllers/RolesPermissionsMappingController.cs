using System.Collections.Generic;
using System.Web.Mvc;
using CPMS.Admin.Presentation;
using CPMS.Authorization;

namespace CPMS.Admin.Delivery.Adapters.Controllers
{
    [Custom.ActionFilters.Authorize(RequiresPermission = PermissionId.EditRolesActivitiesMapping)]
    public class RolesPermissionsMappingController : Controller
    {
        private readonly AdminPresentationService _adminPresentationService;

        public RolesPermissionsMappingController(AdminPresentationService adminPresentationService)
        {
            _adminPresentationService = adminPresentationService;
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Edit()
        {
            return Json(_adminPresentationService.GetRolesPermissionsMapping(), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Edit(IEnumerable<RolePermissionsViewModel> roleViewModels)
        {
            _adminPresentationService.EditRolesPermissionsMapping(roleViewModels);
            return Content("OK", "text/plain");
        }
    }
}
