using System.Web.Http;
using System.Web.Http.ModelBinding;
using CPMS.Authorization;
using CPMS.Authorization.WebAPI;
using CPMS.Infrastructure.WebApi;
using CPMS.Report.Presentation;

namespace CPMS.Report.Delivery.Adapters.Controllers
{
    [Authorization.WebAPI.Authorize(RequiresPermission = PermissionId.EventBreaches)]
    public class EventBreachesController : ApiController
    {
        private readonly ReportPresentationService _reportPresentationService;

        public EventBreachesController(ReportPresentationService reportPresentationService)
        {
            _reportPresentationService = reportPresentationService;
        }

        [HttpGet]
        [ElmahGenericExceptionHandling]
        public EventBreachesViewModel GetEventBreachesReport([ModelBinder(typeof(RoleDataModelBinder))]RoleData role, int daysToBreach, int? index = null, int? pageCount = null, string patientNHSNumber = null, string patientName = null, string eventDescription = null, string hospital = null, string specialty = null, string clinician = null, string periodType = null, int? advancementOrDelay = null, string orderBy = null, string orderDirection = null)
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

            return _reportPresentationService.GetEventBreachesReport(role, daysToBreach, filterInputModel, listInputModel);
        }
    }
}
