using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Data.Entity;
using CPMS.Authorization;
using IUserRepository = CPMS.Admin.Application.IUserRepository;

namespace CPMS.Admin.Core.Adapters
{
    public class UserRepository : IUserRepository
    {
        private readonly UnitOfWork _unitOfWork;

        public UserRepository(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public void Create(User user)
        {
            _unitOfWork.Users.Add(user);
        }

        public IEnumerable<User> Get(Expression<Func<User, bool>> criteria, params Expression<Func<User, object>>[] includeProperties)
        {
            var usersSet = _unitOfWork.Users as IQueryable<User>;

            usersSet = includeProperties.Aggregate(usersSet,
                (current, includeProperty) => current.Include(includeProperty));

            return usersSet.Where(criteria).ToArray();
        }

        public bool Contains(Expression<Func<User, bool>> criteria)
        {
            return _unitOfWork.Users.Any(criteria);
        }
    }
}
