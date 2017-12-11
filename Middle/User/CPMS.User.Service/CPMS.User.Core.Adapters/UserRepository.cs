using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Data.Entity;
using CPMS.Authorization;

namespace CPMS.User.Core.Adapters
{
    public class UserRepository : IUserRepository
    {
        public void Create(Authorization.User user)
        {
            using (var unitOfWork = new UnitOfWork())
            {
                unitOfWork.Users.Add(user);
                unitOfWork.SaveChanges();
            }
        }

        public void Update(Authorization.User user)
        {
            using (var unitOfWork = new UnitOfWork())
            {
                var existingUser = unitOfWork.Users.Single(u => u.Id == user.Id);

                if (existingUser != null)
                {
                    existingUser.Password = user.Password;
                    existingUser.FullName = user.FullName;
                    existingUser.Email = user.Email;
                    existingUser.Username = user.Username;
                    existingUser.IsActive = user.IsActive;
                    existingUser.RefreshToken = user.RefreshToken;

                    unitOfWork.SaveChanges();
                }
            }
        }

        public IEnumerable<Authorization.User> Get(Expression<Func<Authorization.User, bool>> criteria)
        {
            using (var unitOfWork = new UnitOfWork())
            {
                return unitOfWork.Users.Include(usr => usr.Role.Permissions).Where(criteria.Compile()).ToArray();
            }
        }

        public bool Contains(Expression<Func<Authorization.User, bool>> criteria)
        {
            using (var unitOfWork = new UnitOfWork())
            {
                return unitOfWork.Users.Any(criteria.Compile());
            }
        }
    }
}
