using System.Linq;
using System.Web.Mvc;
using CPMS.Admin.Presentation;
using CPMS.Authorization;

namespace CPMS.Admin.Delivery.Adapters.Controllers
{
    [Custom.ActionFilters.Authorize(RequiresPermission = PermissionId.ManageUsers)]
    public class UserController : Controller
    {
        private readonly AdminPresentationService _adminPresentationService;

        public UserController(AdminPresentationService adminPresentationService)
        {
            _adminPresentationService = adminPresentationService;
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult UserList(string fullname = "", string username ="", int jtStartIndex = 0, int jtPageSize = 0, string jtSorting = null)
        {
            var listInputModel = new ListInputModel
            {
                Index = jtStartIndex,
                PageCount = jtPageSize
            };
            if (jtSorting != null)
            {
                var sort = jtSorting.Split(' ');
                listInputModel.OrderBy = sort[0];
                listInputModel.OrderDirection = sort[1];
            }

            var users = _adminPresentationService.GetUsers(fullname, username, listInputModel);

            return Json(new { Result = "OK", Records = users.Users, TotalRecordCount = users.TotalNumberOfUsers });

        }

        [HttpPost]
        public JsonResult UpdateUser(EditUserViewModel userInput)
        {
            if (!ModelState.IsValid)
            {
                var errorlist = (from modelState in ModelState.Values from error in modelState.Errors select error.ErrorMessage).ToList();

                return Json(new
                {
                    Result = "ERROR",
                    Message = "Form is not valid! " + errorlist.FirstOrDefault()
                });
            }
            _adminPresentationService.UpdateUser(userInput);
            return Json(new { Result = "OK" });

        }

        [HttpPost]
        public JsonResult AddUser(AddUserViewModel userInput)
        {
            if (ModelState.IsValid)
            {
                if (!_adminPresentationService.UsernameExists(userInput.Username))
                {
                    _adminPresentationService.CreateUser(userInput);
                    return Json(new { Result = "OK", Record = userInput });
                }

                return Json(new { Result = "ERROR", Message = "Form is not valid! Username is already taken." });
            }

            var errorlist = (from modelState in ModelState.Values from error in modelState.Errors select error.ErrorMessage).ToList();
            return Json(new { Result = "ERROR", Message = "Form is not valid! " + errorlist.FirstOrDefault() });
        }

        [HttpPost]
        public JsonResult GetRoles()
        {
            var roles = _adminPresentationService.GetRoles().Select(r => new { DisplayText = r.Name, Value = r.Id });
            return Json(new { Result = "OK", Options = roles });
        }
    }
}