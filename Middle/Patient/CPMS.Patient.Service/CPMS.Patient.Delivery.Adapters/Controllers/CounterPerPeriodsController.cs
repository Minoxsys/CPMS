using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using CPMS.Authorization;
using CPMS.Authorization.WebAPI;
using CPMS.Infrastructure.WebApi;
using CPMS.Patient.Presentation;

namespace CPMS.Patient.Delivery.Adapters.Controllers
{
    //[Authorization.WebAPI.Authorize(RequiresPermission = PermissionId.RTTPeriodBreaches)]
    public class CounterPerPeriodsController : ApiController
    {
        private readonly PatientPresentationService _patientPresentationService;

        public CounterPerPeriodsController(PatientPresentationService patientPresentationService)
        {
            _patientPresentationService = patientPresentationService;
        }

        [HttpGet]
        [ElmahGenericExceptionHandling]
        public IEnumerable<PeriodCounterViewModel> GetPeriodCounterForPathways([ModelBinder(typeof(RoleDataModelBinder))]RoleData role)
        {
            return _patientPresentationService.GetPeriodCounterForPathways(role);
        }

        [HttpGet]
        [ElmahGenericExceptionHandling]
        public IEnumerable<PeriodCounterViewModel> GetPeriodCounterForGivenPathway([ModelBinder(typeof(RoleDataModelBinder))]RoleData role, string pathwayType)
        {
            return _patientPresentationService.GetPeriodCounterForGivenPathwayType(role, pathwayType);
        }

        [HttpGet]
        [ElmahGenericExceptionHandling]
        public IEnumerable<PeriodCounterViewModel> GetPeriodCounterForGivenHospital([ModelBinder(typeof(RoleDataModelBinder))]RoleData role, string idHospital, string pathwayType)
        {
            return _patientPresentationService.GetPeriodCounterForGivenHospital(role, idHospital, pathwayType);
        }
    }
}