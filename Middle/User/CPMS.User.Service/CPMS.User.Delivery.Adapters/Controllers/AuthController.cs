using System.Web.Http;
using CPMS.User.Delivery.Adapters.Custom.ActionFilters;
using CPMS.User.Presentation;

namespace CPMS.User.Delivery.Adapters.Controllers
{
    public class AuthController : ApiController
    {
        private readonly UserPresentationService _userPresentationService;

        public AuthController(UserPresentationService userPresentationService)
        {
            _userPresentationService = userPresentationService;
        }

        [HttpPost]
        [ElmahUnauthorizedExceptionHandling]
        public AuthenticationViewModel GetAuthenticationInfo(AuthenticationInputModel inputModel)
        {
            return _userPresentationService.GetAuthenticationViewModel(inputModel);
        }

        [HttpPost]
        [ElmahUnauthorizedExceptionHandling]
        public AuthenticationViewModel RefreshAuthenticationInfo(RefreshAuthenticationInputModel inputModel)
        {
            return _userPresentationService.GetAuthenticationViewModel(inputModel);
        }

        [HttpPost]
        [ElmahUnauthorizedExceptionHandling]
        public UserViewModel GetUserInfo(RefreshAuthenticationInputModel inputModel)
        {
            return _userPresentationService.GetUserViewModel(inputModel);
        }

        [HttpPost]
        [ElmahUnauthorizedExceptionHandling]
        public void ChangePassword(ChangePasswordInputModel inputModel)
        {
            _userPresentationService.ChangePassword(inputModel);
        }
    }
}
