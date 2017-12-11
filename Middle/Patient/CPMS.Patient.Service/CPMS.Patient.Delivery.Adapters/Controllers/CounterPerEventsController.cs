using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using CPMS.Authorization;
using CPMS.Authorization.WebAPI;
using CPMS.Infrastructure.WebApi;
using CPMS.Patient.Presentation;

namespace CPMS.Patient.Delivery.Adapters.Controllers
{
    [Authorization.WebAPI.Authorize(RequiresPermission = PermissionId.EventBreaches)]
    public class CounterPerEventsController : ApiController
    {
        private readonly PatientPresentationService _patientPresentationService;

        public CounterPerEventsController(PatientPresentationService patientPresentationService)
        {
            _patientPresentationService = patientPresentationService;
        }

        [HttpGet]
        [ElmahGenericExceptionHandling]
        public IEnumerable<EventCounterViewModel> GetEventsCounter([ModelBinder(typeof(RoleDataModelBinder))]RoleData role)
        {
            return _patientPresentationService.GetEventsCounter(role);
        }
    }
}
