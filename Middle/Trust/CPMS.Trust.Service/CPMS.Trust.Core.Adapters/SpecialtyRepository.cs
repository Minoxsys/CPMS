using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using CPMS.Domain;
using ISpecialtyRepository = CPMS.Trust.Manager.ISpecialtyRepository;

namespace CPMS.Trust.Core.Adapters
{
    public class SpecialtyRepository : ISpecialtyRepository
    {
        private readonly UnitOfWork _unitOfWork;

        public SpecialtyRepository(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IEnumerable<Specialty> Get(Expression<Func<Specialty, bool>> criteria, params Expression<Func<Specialty, object>>[] includeProperties)
        {
            var specialtiesSet = _unitOfWork.Specialties as IQueryable<Specialty>;

            specialtiesSet = includeProperties.Aggregate(specialtiesSet,
                (current, includeProperty) => current.Include(includeProperty));

            return specialtiesSet.Where(criteria).ToArray();
        }
    }
}
