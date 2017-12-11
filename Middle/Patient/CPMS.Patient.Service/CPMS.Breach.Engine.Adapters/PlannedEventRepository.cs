using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

namespace CPMS.Breach.Engine.Adapters
{
    public class PlannedEventRepository : IPlannedEventRepository
    {
        public IEnumerable<PlannedEvent> Get(Expression<Func<PlannedEvent, bool>> criteria, GridInfoModel infoModel)
        {
            using (var unitOfWork = new UnitOfWork())
            {
                if (infoModel == null || (infoModel.Index == null || infoModel.PageCount == null))
                {
                    return GetPlannedEventsByCriteria(criteria, unitOfWork)
                        .ToArray();
                }

                return GetPlannedEventsByCriteria(criteria, unitOfWork)
                        .Skip(infoModel.Index.Value * infoModel.PageCount.Value)
                        .Take(infoModel.PageCount.Value)
                    .ToArray();
            }
        }

        public int Count(Expression<Func<PlannedEvent, bool>> criteria)
        {
            using (var unitOfWork = new UnitOfWork())
            {
                return GetPlannedEventsByCriteria(criteria, unitOfWork).Count();
            }
        }

        private static IEnumerable<PlannedEvent> GetPlannedEventsByCriteria(Expression<Func<PlannedEvent, bool>> criteria, UnitOfWork unitOfWork)
        {
            return unitOfWork.PlannedEvents
                .Include(e => e.Patient)
                .Include(e => e.Event)
                .Include(e => e.Event.Pathway)
                .Where(criteria.Compile());
        }
    }
}
