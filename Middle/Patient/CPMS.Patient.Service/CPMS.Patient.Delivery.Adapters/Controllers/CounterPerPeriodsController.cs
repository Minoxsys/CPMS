using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using CPMS.Authorization;
using CPMS.Authorization.WebAPI;
using CPMS.Infrastructure.WebApi;
using CPMS.Patient.Presentation;

namespace CPMS.Patient.Delivery.Adapters.Controllers
{
    [Authorization.WebAPI.Authorize(RequiresPermission = PermissionId.RTTPeriodBreaches)]
    public class CounterPerPeriodsController : ApiController
    {
        private readonly PatientPresentationService _patientPresentationService;

        public CounterPerPeriodsController(PatientPresentationService patientPresentationService)
        {
            _patientPresentationService = patientPresentationService;
        }

        [HttpGet]
        [ElmahGenericExceptionHandling]
        public IEnumerable<PeriodCounterViewModel> GetPeriodCounterForTrust([ModelBinder(typeof(RoleDataModelBinder))]RoleData role)
        {
            return _patientPresentationService.GetPeriodCounterForTrust(role);
        }

        [HttpGet]
        [ElmahGenericExceptionHandling]
        public IEnumerable<PeriodCounterViewModel> GetPeriodCounterForHospital([ModelBinder(typeof(RoleDataModelBinder))]RoleData role, string hospitalId)
        {
            return _patientPresentationService.GetPeriodCounterForHospital(role, hospitalId);
        }

        [HttpGet]
        [ElmahGenericExceptionHandling]
        public IEnumerable<PeriodCounterViewModel> GetPeriodCounterForSpecialty([ModelBinder(typeof(RoleDataModelBinder))]RoleData role, string specialtyCode, string idHospital = null)
        {
            return _patientPresentationService.GetPeriodCounterForSpecialty(role, specialtyCode, idHospital);
        }
    }
}
