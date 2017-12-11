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
    public class HospitalsController : ApiController
    {
        private readonly TrustPresentationService _trustPresentationService;

        public HospitalsController(TrustPresentationService trustPresentationService)
        {
            _trustPresentationService = trustPresentationService;
        }

        [HttpGet]
        [ElmahGenericExceptionHandling]
        public IEnumerable<HospitalViewModel> Get([ModelBinder(typeof(RoleDataModelBinder))]RoleData role)
        {
            return _trustPresentationService.GetHospitals(role);
        }
    }
}
