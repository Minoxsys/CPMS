namespace CPMS.Admin.Application
{
    public class UserInputInfo
    {
        public int Id { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public string FullName { get; set; }

        public string Email { get; set; }

        public int RoleId { get; set; }

        public bool IsActive { get; set; }
    }
}
