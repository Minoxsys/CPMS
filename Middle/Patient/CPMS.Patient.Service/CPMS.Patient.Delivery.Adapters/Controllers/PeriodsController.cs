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
    public class PeriodsController : ApiController
    {
        private readonly PatientPresentationService _patientPresentationService;

        public PeriodsController(PatientPresentationService patientPresentationService)
        {
            _patientPresentationService = patientPresentationService;
        }

        [HttpGet]
        [ElmahGenericExceptionHandling]
        public IEnumerable<PeriodViewModel> GetPeriodsForPathway([ModelBinder(typeof(RoleDataModelBinder))]RoleData role, string ppiNumber)
        {
            return _patientPresentationService.GetPeriodsForPathway(role, ppiNumber);
        }
    }
}
