using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using CPMS.Authorization;
using CPMS.Authorization.WebAPI;
using CPMS.Infrastructure.WebApi;
using CPMS.Trust.Presentation;

namespace CPMS.Trust.Delivery.Adapters.Controllers
{
    [Authorization.WebAPI.Authorize(RequiresPermission = PermissionId.Trust)]
    public class CliniciansController : ApiController
    {
        private readonly TrustPresentationService _trustPresentationService;

        public CliniciansController(TrustPresentationService trustPresentationService)
        {
            _trustPresentationService = trustPresentationService;
        }

        [HttpGet]
        [ElmahGenericExceptionHandling]
        public IEnumerable<ClinicianViewModel> Get([ModelBinder(typeof(RoleDataModelBinder))]RoleData role, int? hospitalId = null, string specialtyCode = null)
        {
            return _trustPresentationService.GetClinicians(role, hospitalId, specialtyCode);
        }
    }
}
