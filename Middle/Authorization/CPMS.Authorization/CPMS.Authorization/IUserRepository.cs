using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace CPMS.Authorization
{
    public interface IUserRepository
    {
        void Create(User user);

        void Update(User user);

        IEnumerable<User> Get(Expression<Func<User, bool>> criteria);

        bool Contains(Expression<Func<User, bool>> criteria);
    }
}
