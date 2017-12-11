using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using CPMS.Authorization;
using CPMS.Authorization.WebAPI;
using CPMS.Infrastructure.WebApi;
using CPMS.Report.Presentation;

namespace CPMS.Report.Delivery.Adapters.Controllers
{
    [Authorization.WebAPI.Authorize(RequiresPermission = PermissionId.EventBreaches)]
    public class EventPerformanceController : ApiController
    {
        private readonly ReportPresentationService _reportPresentationService;

        public EventPerformanceController(ReportPresentationService reportPresentationService)
        {
            _reportPresentationService = reportPresentationService;
        }

        [HttpGet]
        [ElmahGenericExceptionHandling]
        public IEnumerable<EventPerformanceViewModel> GetEventPerformanceReport([ModelBinder(typeof(RoleDataModelBinder))] RoleData role)
        {
            return _reportPresentationService.GetEventPerformanceReport(role);
        }
    }
}
