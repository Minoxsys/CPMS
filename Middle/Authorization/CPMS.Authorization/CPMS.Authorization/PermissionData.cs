using System;

namespace CPMS.Authorization
{
    [Serializable]
    public class PermissionData
    {
        public PermissionId Id { get; set; }

        public string Name { get; set; }
    }
}
