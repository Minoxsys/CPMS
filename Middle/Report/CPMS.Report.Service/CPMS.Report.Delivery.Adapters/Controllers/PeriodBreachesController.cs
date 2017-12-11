using System.Web.Http;
using System.Web.Http.ModelBinding;
using CPMS.Authorization;
using CPMS.Authorization.WebAPI;
using CPMS.Infrastructure.WebApi;
using CPMS.Report.Presentation;

namespace CPMS.Report.Delivery.Adapters.Controllers
{
    [Authorization.WebAPI.Authorize(RequiresPermission = PermissionId.RTTPeriodBreaches)]
    public class PeriodBreachesController : ApiController
    {
        private readonly ReportPresentationService _reportPresentationService;

        public PeriodBreachesController(ReportPresentationService reportPresentationService)
        {
            _reportPresentationService = reportPresentationService;
        }

        [HttpGet]
        [ElmahGenericExceptionHandling]
        public PeriodBreachesViewModel GetPeriodBreachesReport([ModelBinder(typeof(RoleDataModelBinder))]RoleData role, int weeksToBreach, int? index = null, int? pageCount = null, string patientNHSNumber = null, string patientName = null, string eventDescription = null, string hospital = null, string specialty = null, string clinician = null, string periodType = null, int? advancementOrDelay = null, string orderBy = null, string orderDirection = null)
        {
            var filterInputModel = new BreachFilterInputModel
            {
                EventDescription = eventDescription,
                NhsNumber = patientNHSNumber,
                PatientName = patientName,
                Specialty = specialty,
                Clinician = clinician,
                Hospital = hospital,
                PeriodType = periodType,
                AdvancementOrDelay = advancementOrDelay
            };

            var listInputModel = new ListInputModel
            {
                Index = index,
                PageCount = pageCount,
                OrderBy = orderBy,
                OrderDirection = orderDirection
            };

            return _reportPresentationService.GetPeriodBreachesReport(role, weeksToBreach, filterInputModel, listInputModel);
        }
    }
}
