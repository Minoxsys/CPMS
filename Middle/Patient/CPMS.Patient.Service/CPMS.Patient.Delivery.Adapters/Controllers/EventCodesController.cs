using System.Collections.Generic;
using System.Web.Http;
using CPMS.Authorization;
using CPMS.Infrastructure.WebApi;
using CPMS.Patient.Presentation;

namespace CPMS.Patient.Delivery.Adapters.Controllers
{
    [Authorization.WebAPI.Authorize(RequiresPermission = PermissionId.Trust)]
    public class EventCodesController : ApiController
    {
        private readonly PatientPresentationService _patientPresentationService;

        public EventCodesController(PatientPresentationService patientPresentationService)
        {
            _patientPresentationService = patientPresentationService;
        }

        [HttpGet]
        [ElmahGenericExceptionHandling]
        public IEnumerable<EventCodeViewModel> Get()
        {
            return _patientPresentationService.GetEventCodes();
        }
    }
}
