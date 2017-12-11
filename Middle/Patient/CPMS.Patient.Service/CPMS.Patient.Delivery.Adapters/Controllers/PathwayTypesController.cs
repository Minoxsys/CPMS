using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using CPMS.Authorization;
using CPMS.Authorization.WebAPI;
using CPMS.Infrastructure.WebApi;
using CPMS.Patient.Manager;

namespace CPMS.Patient.Delivery.Adapters.Controllers
{
    [Authorization.WebAPI.Authorize(RequiresPermission = PermissionId.Patient)]
    public class PathwayTypesController : ApiController
    {
        private readonly PatientApplicationService _patientApplicationServiceService;

        public PathwayTypesController(PatientApplicationService patientApplicationService)
        {
            _patientApplicationServiceService = patientApplicationService;
        }

        [HttpGet]
        [ElmahGenericExceptionHandling]
        public IEnumerable<string> GetPathwayTypes([ModelBinder(typeof(RoleDataModelBinder))]RoleData role)
        {
            return _patientApplicationServiceService.GetPathwayTypes();
        }
    }
}