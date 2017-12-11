using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using CPMS.Authorization;
using CPMS.Authorization.WebAPI;
using CPMS.Infrastructure.WebApi;
using CPMS.Report.Presentation;

namespace CPMS.Report.Delivery.Adapters.Controllers
{
    [Authorization.WebAPI.Authorize(RequiresPermission = PermissionId.RTTPeriodBreaches)]
    public class PeriodPerformanceController : ApiController
    {
        private readonly ReportPresentationService _reportPresentationService;

        public PeriodPerformanceController(ReportPresentationService reportPresentationService)
        {
            _reportPresentationService = reportPresentationService;
        }

        [HttpGet]
        [ElmahGenericExceptionHandling]
        public IEnumerable<PeriodPerformanceViewModel> GetPeriodPerformanceReport([ModelBinder(typeof(RoleDataModelBinder))] RoleData role)
        {
            return _reportPresentationService.GetPeriodPerformanceReport(role);
        }

        [HttpGet]
        [ElmahGenericExceptionHandling]
        public IEnumerable<PeriodPerformanceViewModel> GetPeriodPerformanceReport([ModelBinder(typeof(RoleDataModelBinder))] RoleData role, string hospitalId)
        {
            return _reportPresentationService.GetPeriodPerformanceReport(role, hospitalId);
        }

        [HttpGet]
        [ElmahGenericExceptionHandling]
        public IEnumerable<PeriodPerformanceViewModel> GetPeriodPerformanceReport([ModelBinder(typeof(RoleDataModelBinder))] RoleData role, string hospitalId, string specialtyCode)
        {
            return _reportPresentationService.GetPeriodPerformanceReport(role, hospitalId, specialtyCode);
        }
    }
}
