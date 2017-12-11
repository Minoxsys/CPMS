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
    public class SpecialtiesController : ApiController
    {
        private readonly TrustPresentationService _trustPresentationService;

        public SpecialtiesController(TrustPresentationService trustPresentationService)
        {
            _trustPresentationService = trustPresentationService;
        }

        [HttpGet]
        [ElmahGenericExceptionHandling]
        public IEnumerable<SpecialtyViewModel> Get([ModelBinder(typeof(RoleDataModelBinder))]RoleData role, int? hospitalId = null)
        {
            return _trustPresentationService.GetSpecialties(role, hospitalId);
        }
    }
}
