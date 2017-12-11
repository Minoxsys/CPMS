using System.Collections.Generic;

namespace CPMS.Admin.Presentation
{
    public class UsersViewModel
    {
        public IEnumerable<UserViewModel> Users { get; set; }

        public int TotalNumberOfUsers { get; set; }
    }
}
