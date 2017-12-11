using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using CPMS.Patient.Domain;

namespace CPMS.Notification.Core.Adapters
{
    public class PeriodRepository : IPeriodRepository
    {
        public IEnumerable<Period> Get(Expression<Func<Period, bool>> criteria)
        {
            using (var unitOfWork = new UnitOfWork())
            {
                return GetPeriodsByCriteria(criteria, unitOfWork).ToArray();
            }
        }

        public IEnumerable<Period> Get(Expression<Func<Period, bool>> criteria, DateTime fromDate)
        {
            using (var unitOfWork = new UnitOfWork())
            {
                return GetPeriodsByCriteria(criteria, unitOfWork).ToArray();
            }
        }

        public int Count(Expression<Func<Period, bool>> criteria)
        {
            using (var unitOfWork = new UnitOfWork())
            {
                return GetPeriodsByCriteria(criteria, unitOfWork).Count();
            }
        }

        private IEnumerable<Period> GetPeriodsByCriteria(Expression<Func<Period, bool>> criteria, UnitOfWork unitOfWork)
        {
            return unitOfWork.Periods
                .Include(p => p.Pathway)
                .Include(p => p.Events.Select(e => e.Clinician.Specialty))
                .Include(p => p.Events.Select(e => e.Clinician.Hospital))
                .Where(criteria.Compile());
        }
    }
}
