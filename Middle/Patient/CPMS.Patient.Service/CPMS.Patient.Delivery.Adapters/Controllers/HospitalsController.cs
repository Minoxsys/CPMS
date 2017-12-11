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
    public class HospitalsController : ApiController
    {
        private readonly PatientPresentationService _patientPresentationService;

        public HospitalsController(PatientPresentationService patientPresentationService)
        {
            _patientPresentationService = patientPresentationService;
        }

        [HttpGet]
        [ElmahGenericExceptionHandling]
        public IEnumerable<HospitalViewModel> Get([ModelBinder(typeof(RoleDataModelBinder))]RoleData role, string pathwayType = null)
        {
            return _patientPresentationService.GetHospitals(role, pathwayType);
        }
    }
}
