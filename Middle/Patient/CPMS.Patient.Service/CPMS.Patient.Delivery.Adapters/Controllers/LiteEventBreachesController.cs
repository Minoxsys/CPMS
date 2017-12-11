using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using CPMS.Authorization;
using CPMS.Authorization.WebAPI;
using CPMS.Infrastructure.WebApi;
using CPMS.Patient.Presentation;

namespace CPMS.Patient.Delivery.Adapters.Controllers
{
    [Authorization.WebAPI.Authorize(RequiresPermission = PermissionId.Patient)]
    public class LiteEventBreachesController : ApiController
    {
        private readonly PatientPresentationService _patientPresentationService;

        public LiteEventBreachesController(PatientPresentationService patientPresentationService)
        {
            _patientPresentationService = patientPresentationService;
        }

        [HttpGet]
        [ElmahGenericExceptionHandling]
        public IEnumerable<LiteEventBreachViewModel> GetLiteEventBreaches([ModelBinder(typeof(RoleDataModelBinder))]RoleData role, int periodId)
        {
            return _patientPresentationService.GetLiteEventBreaches(role, periodId);
        }
    }
}
