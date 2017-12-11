using System.Web.Http;
using System.Web.Http.ModelBinding;
using CPMS.Authorization;
using CPMS.Authorization.WebAPI;
using CPMS.Infrastructure.WebApi;
using CPMS.Patient.Presentation;

namespace CPMS.Patient.Delivery.Adapters.Controllers
{
    public class PeriodEventsController : ApiController
    {
        private readonly PatientPresentationService _patientPresentationService;

        public PeriodEventsController(PatientPresentationService patientPresentationService)
        {
            _patientPresentationService = patientPresentationService;
        }

        [HttpGet]
        [ElmahGenericExceptionHandling]
        [Authorization.WebAPI.Authorize(RequiresPermission = PermissionId.Patient)]
        public PeriodEventsViewModel GetPeriodEvents([ModelBinder(typeof(RoleDataModelBinder))]RoleData role, int periodId, int? index = null, int? pageCount = null, string eventCode = null, string specialty = null, int? targetYear = null, int? actualYear = null, int? breaches = null, string periodType = null, string orderBy = null, string orderDirection = null)
        {
            var periodEventsInputModel = new PeriodEventsInputModel
            {
                PeriodEventsFilterInputModel = new PeriodEventsFilterInputModel
                {
                    EventCode = eventCode,
                    Specialty = specialty,
                    TargetYear = targetYear,
                    ActualYear = actualYear,
                    Breaches = breaches,
                    PeriodType = periodType
                },
                ListInputModel = new ListInputModel
                {
                    Index = index,
                    PageCount = pageCount,
                    OrderBy = orderBy,
                    OrderDirection = orderDirection
                }
            };

            return _patientPresentationService.GetPeriodEvents(role, periodId, periodEventsInputModel);
        }

        [HttpGet]
        [ElmahGenericExceptionHandling]
        [Authorization.WebAPI.Authorize]
        public PeriodsAndEventsBreachesCountViewModel GetPeriodsAndEventsBreachesCount([ModelBinder(typeof(RoleDataModelBinder))]RoleData role)
        {
            return _patientPresentationService.GetPeriodsAndEventsBreachesCount(role);
        }
    }
}
