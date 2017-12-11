namespace CPMS.User.Presentation
{
    public class UserViewModel
    {
        public string Username { get; set; }

        public string FullName { get; set; }

        public string Email { get; set; }

        public RoleViewModel Role { get; set; }
    }
}
