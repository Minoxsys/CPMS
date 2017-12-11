using System.Web.Mvc;
using CPMS.Admin.Delivery.Adapters.Custom.ActionFilters;
using CPMS.Admin.Presentation;
using CPMS.Authorization;

namespace CPMS.Admin.Delivery.Adapters.Controllers
{
    [Custom.ActionFilters.Authorize(RequiresPermission = PermissionId.ManageEventMilestones)]
    public class EventMilestoneController : Controller
    {
        private readonly AdminPresentationService _adminPresentationService;

        public EventMilestoneController(AdminPresentationService adminPresentationService)
        {
            _adminPresentationService = adminPresentationService;
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult EventMilestoneList(string parentEvent = "", string eventMilestone = "", string eventForTarget = "", int jtStartIndex = 0, int jtPageSize = 0, string jtSorting = null)
        {
            var listInputModel = new ListInputModel
            {
                Index = jtStartIndex,
                PageCount = jtPageSize
            };
            var eventMilestoneFilterInputModel = new EventMilestoneFilterInputModel
            {
                ParentEventDescription = parentEvent,
                EventMilestoneDescription = eventMilestone,
                EventForTargetDescription = eventForTarget
            };
            
            if (jtSorting != null)
            {
                var sort = jtSorting.Split(' ');
                listInputModel.OrderBy = sort[0];
                listInputModel.OrderDirection = sort[1];
            }

            var eventMilestones = _adminPresentationService.GetEventMilestones(listInputModel, eventMilestoneFilterInputModel);
            return Json(new { Result = "OK", Records = eventMilestones.EventMilestones, TotalRecordCount = eventMilestones.TotalNumberOfEventMilestones });
        }

        [HttpPost]
        [SaveUnitOfWorkChanges]
        public JsonResult UpdateEventMilestone(EventMilestoneViewModel eventMilestone)
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
            _adminPresentationService.UpdateEventMilestone(eventMilestone);
            return Json(new { Result = "OK" });

        }
    }
}