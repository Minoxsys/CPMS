using System.Web.Mvc;
using CPMS.Admin.Presentation;
using CPMS.Authorization;

namespace CPMS.Admin.Delivery.Adapters.Controllers
{
    [Custom.ActionFilters.Authorize(RequiresPermission = PermissionId.ManagePlannedEvents)]
    public class PlannedEventController : Controller
    {
        private readonly AdminPresentationService _adminPresentationService;

        public PlannedEventController(AdminPresentationService adminPresentationService)
        {
            _adminPresentationService = adminPresentationService;
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult PlannedEventList(string parentEvent = "", string plannedEvent = "", string eventForTarget = "", int jtStartIndex = 0, int jtPageSize = 0, string jtSorting = null)
        {
            var listInputModel = new ListInputModel
            {
                Index = jtStartIndex,
                PageCount = jtPageSize
            };
            var plannedEventFilterInputModel = new PlannedEventFilterInputModel
            {
                ParentEventValue = parentEvent,
                PlannedEventValue = plannedEvent,
                EventForTargetValue = eventForTarget
            };
            
            if (jtSorting != null)
            {
                var sort = jtSorting.Split(' ');
                listInputModel.OrderBy = sort[0];
                listInputModel.OrderDirection = sort[1];
            }

            var events = _adminPresentationService.GetPlannedEvents(listInputModel, plannedEventFilterInputModel);
            return Json(new { Result = "OK", Records = events.PlannedEvents, TotalRecordCount = events.TotalNumberOfEvents });

        }

        [HttpPost]
        public JsonResult UpdatePlannedEvent(PlannedEventViewModel plannedEvent)
        {
            if (!ModelState.IsValid)
            {
                return Json(new
                {
                    Result = "ERROR",
                    Message = "Form is not valid! " +
                      "Please correct it and try again."
                });
            }
            _adminPresentationService.UpdatePlannedEvent(plannedEvent);
            return Json(new { Result = "OK" });

        }
    }
}