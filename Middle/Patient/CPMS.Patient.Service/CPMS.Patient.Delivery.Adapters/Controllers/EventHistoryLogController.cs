using System.Web.Http;
using System.Web.Http.ModelBinding;
using CPMS.Authorization;
using CPMS.Authorization.WebAPI;
using CPMS.Infrastructure.WebApi;
using CPMS.Patient.Presentation;

namespace CPMS.Patient.Delivery.Adapters.Controllers
{
    [Authorization.WebAPI.Authorize(RequiresPermission = PermissionId.Patient)]
    public class EventHistoryLogController : ApiController
    {
        private readonly PatientPresentationService _patientPresentationService;

        public EventHistoryLogController(PatientPresentationService patientPresentationService)
        {
            _patientPresentationService = patientPresentationService;
        }

        [HttpGet]
        [ElmahGenericExceptionHandling]
        public EventsHistoryLogViewModel GetEventHistoryLog([ModelBinder(typeof(RoleDataModelBinder))]RoleData role, int periodId, int? importYear = null, string eventCode = null, string description = null, int? targetYear = null, int? actualYear = null, int? index = null, int? pageCount = null, string orderBy = null, string orderDirection = null)
        {
            var inputModel = new EventHistoryLogInputModel
            {
                EventHistoryLogFilterInputModel = new EventHistoryLogFilterInputModel
                {
                    ImportYear = importYear,
                    EventCode = eventCode,
                    Description = description,
                    TargetYear = targetYear,
                    ActualYear = actualYear
                },
                ListInputModel = new ListInputModel
                {
                    Index = index,
                    PageCount = pageCount,
                    OrderDirection = orderDirection,
                    OrderBy = orderBy
                }
            };
            return _patientPresentationService.GetEventHistoryLog(role, periodId, inputModel);
        }
    }
}
