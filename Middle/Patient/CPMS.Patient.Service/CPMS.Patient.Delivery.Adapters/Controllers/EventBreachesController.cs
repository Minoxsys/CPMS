using System.Web.Http;
using System.Web.Http.ModelBinding;
using CPMS.Authorization;
using CPMS.Authorization.WebAPI;
using CPMS.Infrastructure.WebApi;
using CPMS.Patient.Presentation;

namespace CPMS.Patient.Delivery.Adapters.Controllers
{
    [Authorization.WebAPI.Authorize(RequiresPermission = PermissionId.EventBreaches)]
    public class EventBreachesController : ApiController
    {
        private readonly PatientPresentationService _patientPresentationService;

        public EventBreachesController(PatientPresentationService patientPresentationService)
        {
            _patientPresentationService = patientPresentationService;
        }

        [HttpGet]
        [ElmahGenericExceptionHandling]
        public EventBreachesViewModel GetEventBreaches([ModelBinder(typeof(RoleDataModelBinder))]RoleData role, int daysToBreach, int? index = null, int? pageCount = null, string patientNHSNumber = null, string patientName = null, string eventCode = null, string hospital = null, string specialty = null, string clinician = null, string periodType = null, int? advancementOrDelay = null, string orderBy = null, string orderDirection = null)
        {
            var breachInputModel = new BreachInputModel
            {
                BreachFilterInputModel = new BreachFilterInputModel
                {
                    EventCode = eventCode,
                    NhsNumber = patientNHSNumber,
                    PatientName = patientName,
                    Specialty = specialty,
                    Clinician = clinician,
                    Hospital = hospital,
                    PeriodType = periodType,
                    AdvancementOrDelay = advancementOrDelay
                },
                ListInputModel = new ListInputModel
                {
                    Index = index,
                    PageCount = pageCount,
                    OrderBy = orderBy,
                    OrderDirection = orderDirection
                }
            };
            return _patientPresentationService.GetEventBreaches(role, daysToBreach, breachInputModel);
        }
    }
}
