namespace CPMS.User.Presentation
{
    public class ChangePasswordInputModel
    {
        public string Username { get; set; }

        public string OldPassword { get; set; }

        public string NewPassword { get; set; }
    }
}
