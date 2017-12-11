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
    public class PathwaysController : ApiController
    {
        private readonly PatientPresentationService _patientPresentationService;

        public PathwaysController(PatientPresentationService patientPresentationService)
        {
            _patientPresentationService = patientPresentationService;
        }

        [HttpGet]
        [ElmahGenericExceptionHandling]
        public IEnumerable<PathwayViewModel> GetPathwaysForPatient([ModelBinder(typeof(RoleDataModelBinder))]RoleData role, string nhsNumber)
        {
            return _patientPresentationService.GetPathwaysForPatient(role, nhsNumber);
        }
    }
}
