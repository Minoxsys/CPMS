using System.Web.Http;
using System.Web.Http.ModelBinding;
using CPMS.Authorization;
using CPMS.Authorization.WebAPI;
using CPMS.Infrastructure.WebApi;
using CPMS.Report.Presentation;

namespace CPMS.Report.Delivery.Adapters.Controllers
{
    [Authorization.WebAPI.Authorize]
    public class PeriodsAndEventsPerformanceController : ApiController
    {
        private readonly ReportPresentationService _reportPresentationService;

        public PeriodsAndEventsPerformanceController(ReportPresentationService reportPresentationService)
        {
            _reportPresentationService = reportPresentationService;
        }

        [HttpGet]
        [ElmahGenericExceptionHandling]
        public PeriodsAndEventsPerformanceViewModel GetPeriodsAndEventsPerformanceReport([ModelBinder(typeof(RoleDataModelBinder))] RoleData role)
        {
            return _reportPresentationService.GetPeriodsAndEventsPerformanceReport(role);
        }
    }
}
