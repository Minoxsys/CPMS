using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using CPMS.Authorization;
using System.Data.Entity;

namespace CPMS.Admin.Core.Adapters
{
    public class UserRepository : IUserRepository
    {
        public void Create(User user)
        {
            using (var unitOfWork = new UnitOfWork())
            {
                var newUser = new User
                {
                    FullName = user.FullName,
                    Email = user.Email,
                    Username = user.Username,
                    Password = user.Password,
                    Role = unitOfWork.Roles.Single(r => r.Id == user.Role.Id),
                    IsActive = user.IsActive
                };
                unitOfWork.Users.Add(newUser);
                unitOfWork.SaveChanges();
            }
        }

        public void Update(User user)
        {
            using (var unitOfWork = new UnitOfWork())
            {
                var existingUser = unitOfWork.Users.Single(u => u.Id == user.Id);

                if (user.Password != null)
                {
                    existingUser.Password = user.Password;
                }
                existingUser.FullName = user.FullName;
                existingUser.Email = user.Email;
                existingUser.Username = user.Username;
                existingUser.Role = unitOfWork.Roles.Single(r => r.Id == user.Role.Id);
                existingUser.IsActive = user.IsActive;

                unitOfWork.SaveChanges();
            }
        }

        public IEnumerable<User> Get(Expression<Func<User, bool>> criteria)
        {
            using (var unitOfWork = new UnitOfWork())
            {
                return unitOfWork.Users.Include(u => u.Role).Where(criteria.Compile()).ToArray();
            }
        }

        public bool Contains(Expression<Func<User, bool>> criteria)
        {
            using (var unitOfWork = new UnitOfWork())
            {
                return unitOfWork.Users.Any(criteria.Compile());
            }
        }
    }
}
