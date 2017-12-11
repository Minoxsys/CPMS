using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using CPMS.Authorization;

namespace CPMS.Admin.Application
{
    public interface IUserRepository
    {
        void Create(User user);

        IEnumerable<User> Get(Expression<Func<User, bool>> criteria, params Expression<Func<User, object>>[] includeProperties);

        bool Contains(Expression<Func<User, bool>> criteria);
    }
}
