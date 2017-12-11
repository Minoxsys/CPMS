using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using CPMS.Authorization;
using CPMS.Authorization.WebAPI;
using CPMS.Infrastructure.WebApi;
using CPMS.Patient.Presentation;

namespace CPMS.Patient.Delivery.Adapters.Controllers
{
    [Authorization.WebAPI.Authorize(RequiresPermission = PermissionId.Trust)]
    public class SpecialtiesController : ApiController
    {
        private readonly PatientPresentationService _patientPresentationService;

        public SpecialtiesController(PatientPresentationService patientPresentationService)
        {
            _patientPresentationService = patientPresentationService;
        }

        [HttpGet]
        [ElmahGenericExceptionHandling]
        public IEnumerable<SpecialtyViewModel> Get([ModelBinder(typeof(RoleDataModelBinder))]RoleData role, int? hospitalId = null)
        {
            return _patientPresentationService.GetSpecialties(role, hospitalId);
        }
    }
}
