using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace CPMS.Authorization
{
    public interface IRoleRepository
    {
        void Create(Role role);

        void Update(Role role);

        IEnumerable<Role> Get(Expression<Func<Role, bool>> criteria);
    }
}
