namespace CPMS.Admin.Presentation
{
    public class UserViewModel
    {
        public int Id { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public string ConfirmPassword { get; set; }

        public string FullName { get; set; }

        public string Email { get; set; }

        public int RoleId { get; set; }

        public bool IsActive { get; set; }
    }
}
