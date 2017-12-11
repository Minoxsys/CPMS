using System.ComponentModel.DataAnnotations;

namespace CPMS.Admin.Presentation
{
    public class RoleViewModel
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }
    }
}
