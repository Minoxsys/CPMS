using System.ComponentModel.DataAnnotations;

namespace CPMS.Admin.Presentation
{
    public class EditUserViewModel
    {
        public int Id { get; set; }

        public string Username { get; set; }

        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 4)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Confim Password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [Required, MaxLength(30)]
        [Display(Name = "Full Name")]
        public string FullName { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email Address")]
        public string Email { get; set; }

        [Display(Name = "Active")]
        public bool IsActive { get; set; }

        public int RoleId { get; set; }
    }
}
