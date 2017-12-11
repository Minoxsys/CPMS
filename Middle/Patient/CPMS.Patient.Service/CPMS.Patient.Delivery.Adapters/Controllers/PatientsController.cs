using System.Web.Http;
using System.Web.Http.ModelBinding;
using CPMS.Authorization;
using CPMS.Authorization.WebAPI;
using CPMS.Infrastructure.WebApi;
using CPMS.Patient.Presentation;

namespace CPMS.Patient.Delivery.Adapters.Controllers
{
    [Authorization.WebAPI.Authorize(RequiresPermission = PermissionId.Patient)]
    public class PatientsController : ApiController
    {
        private readonly PatientPresentationService _patientPresentationService;

        public PatientsController(PatientPresentationService patientPresentationService)
        {
            _patientPresentationService = patientPresentationService;
        }

        [HttpGet]
        [ElmahGenericExceptionHandling]
        public PatientsViewModel GetPatientsOnPathway([ModelBinder(typeof(RoleDataModelBinder))]RoleData role, int? index = null, int? pageCount = null, string patientName = null, string hospital = null, string ppiNumber = null, string nhsNumber = null, string periodType = null, string orderBy = null, string orderDirection = null, string pathwayType = null)
        {
            var patiensInputModel = new PatientInputModel
            {
                PatientsFilterInputModel = new PatientFilterInputModel
                {
                    PatientName = patientName,
                    Hospital = hospital,
                    PpiNumber = ppiNumber,
                    NhsNumber = nhsNumber,
                    PeriodType = periodType,
                    PathwayType = pathwayType
                },
                ListInputModel = new ListInputModel
                {
                    Index = index,
                    PageCount = pageCount,
                    OrderBy = orderBy,
                    OrderDirection = orderDirection
                }
            };

            return _patientPresentationService.GetPatientsOnPathway(role, patiensInputModel);
        }
    }
}
