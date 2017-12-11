using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using CPMS.Domain;
using CPMS.Patient.Manager;

namespace CPMS.Core.Adapters
{
    public class PeriodRepository : IPeriodRepository
    {
        private readonly UnitOfWork _unitOfWork;

        public PeriodRepository(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IEnumerable<Period> Get(Expression<Func<Period, bool>> criteria, params Expression<Func<Period, object>>[] includeProperties)
        {
            return GetPeriodsByCriteria(criteria, includeProperties).ToArray();
        }

        public int Count(Expression<Func<Period, bool>> criteria)
        {
            return GetPeriodsByCriteria(criteria).Count();
        }

        private IEnumerable<Period> GetPeriodsByCriteria(Expression<Func<Period, bool>> criteria, params Expression<Func<Period, object>>[] includeProperties)
        {
            var periodsSet = _unitOfWork.Periods as IQueryable<Period>;

            periodsSet = includeProperties.Aggregate(periodsSet, (current, includeProperty) => current.Include(includeProperty));

            return periodsSet
                .Where(criteria);
        }
    }
}
