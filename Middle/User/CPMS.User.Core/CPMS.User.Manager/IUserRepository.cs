using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace CPMS.User.Manager
{
    public interface IUserRepository
    {
        IEnumerable<Authorization.User> Get(Expression<Func<Authorization.User, bool>> criteria, params Expression<Func<Authorization.User, object>>[] includeProperties);
    }
}
