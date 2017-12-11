using System.Collections.Generic;

namespace CPMS.User.Presentation
{
    public class RoleViewModel
    {
        public string Name { get; set; }

        public IEnumerable<string> Permissions { get; set; }
    }
}
