using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using CPMS.Patient.Domain;

namespace CPMS.Notification.Core.Adapters
{
    public class PlannedEventRepository : IPlannedEventRepository
    {
        public IEnumerable<PlannedEvent> Get(Expression<Func<PlannedEvent, bool>> criteria, ListInput infoModel)
        {
            using (var unitOfWork = new UnitOfWork())
            {
                return GetPlannedEventsByCriteria(criteria, unitOfWork).ToArray();
            }
        }

        public int Count(Expression<Func<PlannedEvent, bool>> criteria)
        {
            using (var unitOfWork = new UnitOfWork())
            {
                return GetPlannedEventsByCriteria(criteria, unitOfWork).Count();
            }
        }

        private IEnumerable<PlannedEvent> GetPlannedEventsByCriteria(Expression<Func<PlannedEvent, bool>> criteria, UnitOfWork unitOfWork)
        {
            return unitOfWork.PlannedEvents
                .Include(e => e.Event.Period.Pathway.Patient)
                .Where(criteria.Compile());
        }
    }
}
