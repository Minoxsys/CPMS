using System;
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
    public class PeriodBreachesController : ApiController
    {
        private readonly PatientPresentationService _patientPresentationService;

        public PeriodBreachesController(PatientPresentationService patientPresentationService)
        {
            _patientPresentationService = patientPresentationService;
        }

        [HttpGet]
        [ElmahGenericExceptionHandling]
        public PeriodBreachesViewModel GetPeriodBreaches([ModelBinder(typeof(RoleDataModelBinder))]RoleData role, int weeksToBreach, int? index = null, int? pageCount = null, string patientNHSNumber = null, string patientName = null, string eventCode = null, string hospital = null, string specialty = null, string clinician = null, string periodType = null, string orderBy = null, string orderDirection = null)
        {
            var breachInputModel = new BreachInputModel
            {
                BreachFilterInputModel = new BreachFilterInputModel
                {
                    EventCode = eventCode,
                    NhsNumber = patientNHSNumber,
                    PatientName = patientName,
                    Specialty = specialty,
                    Hospital = hospital,
                    Clinician = clinician,
                    PeriodType = periodType
                },
                ListInputModel = new ListInputModel
                {
                    Index = index,
                    PageCount = pageCount,
                    OrderBy = orderBy,
                    OrderDirection = orderDirection
                }
            };
            return _patientPresentationService.GetPeriodBreaches(role, weeksToBreach, breachInputModel);
        }
    }
}
