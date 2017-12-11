using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using CPMS.Domain;
using CPMS.Report.Manager;

namespace CPMS.Report.Core.Adapters
{
    public class PeriodRepository : IPeriodRepository
    {
        public IEnumerable<Period> Get(Expression<Func<Period, bool>> criteria, DateTime? fromDate = null, params Expression<Func<Period, object>>[] includeProperties)
        {
            using (var unitOfWork = new UnitOfWork())
            {
                var completedPeriodsSet = unitOfWork.Periods as IQueryable<Period>;

                completedPeriodsSet = includeProperties.Aggregate(completedPeriodsSet,
                    (current, includeProperty) => current.Include(includeProperty));

                return completedPeriodsSet
                    .Where(criteria)
                    .Where(p => fromDate == null || DbFunctions.AddDays(p.StartDate, p.MaximumNumberOfDays) >= fromDate)
                    .ToArray();
            }
        }

        public int Count(Expression<Func<Period, bool>> criteria)
        {
            using (var unitOfWork = new UnitOfWork())
            {
                return unitOfWork.Periods.Count(criteria);
            }
        }
    }
}
