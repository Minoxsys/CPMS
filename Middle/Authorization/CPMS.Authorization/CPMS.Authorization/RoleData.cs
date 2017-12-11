using System;
using System.Collections.Generic;

namespace CPMS.Authorization
{
    [Serializable]
    public class RoleData
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public IList<PermissionData> Permissions { get; set; }
    }
}
