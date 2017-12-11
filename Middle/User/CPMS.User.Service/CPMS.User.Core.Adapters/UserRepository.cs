using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Data.Entity;
using CPMS.User.Manager;

namespace CPMS.User.Core.Adapters
{
    public class UserRepository : IUserRepository
    {
        private readonly UnitOfWork _unitOfWork;

        public UserRepository(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IEnumerable<Authorization.User> Get(Expression<Func<Authorization.User, bool>> criteria, params Expression<Func<Authorization.User, object>>[] includeProperties)
        {
            var usersSet = _unitOfWork.Users as IQueryable<Authorization.User>;

            usersSet = includeProperties.Aggregate(usersSet,
                (current, includeProperty) => current.Include(includeProperty));

            return usersSet.Where(criteria).ToArray();
        }
    }
}
