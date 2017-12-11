using CPMS.User.Manager;

namespace CPMS.User.Presentation
{
    public class UserPresentationService
    {
        private readonly IMapper<AuthenticationInfo, AuthenticationViewModel> _authenticationInfoToViewModelMapper;
        private readonly IMapper<UserInfo, UserViewModel> _userInfoToViewModelMapper;
        private readonly UserApplicationService _userApplicationService;

        public UserPresentationService(
            IMapper<AuthenticationInfo, AuthenticationViewModel> authenticationInfoToViewModelMapper,
            IMapper<UserInfo, UserViewModel> userInfoToViewModelMapper,
            UserApplicationService userApplicationService)
        {
            _authenticationInfoToViewModelMapper = authenticationInfoToViewModelMapper;
            _userInfoToViewModelMapper = userInfoToViewModelMapper;
            _userApplicationService = userApplicationService;
        }

        public AuthenticationViewModel GetAuthenticationViewModel(AuthenticationInputModel inputModel)
        {
            return _authenticationInfoToViewModelMapper.Map(_userApplicationService.GetAuthenticationInfo(inputModel.Username, inputModel.Password));
        }

        public AuthenticationViewModel GetAuthenticationViewModel(RefreshAuthenticationInputModel inputModel)
        {
            return _authenticationInfoToViewModelMapper.Map(_userApplicationService.GetAuthenticationInfo(inputModel.RefreshToken));
        }

        public UserViewModel GetUserViewModel(RefreshAuthenticationInputModel inputModel)
        {
            return _userInfoToViewModelMapper.Map(_userApplicationService.GetUserInfo(inputModel.RefreshToken));
        }

        public void ChangePassword(ChangePasswordInputModel inputModel)
        {
            _userApplicationService.ChangePassword(inputModel.Username, inputModel.OldPassword,
                inputModel.NewPassword);
        }
    }
}
