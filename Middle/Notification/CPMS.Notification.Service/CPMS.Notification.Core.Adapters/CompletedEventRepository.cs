using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using CPMS.Domain;
using ICompletedEventRepository = CPMS.Notification.Manager.ICompletedEventRepository;

namespace CPMS.Notification.Core.Adapters
{
    public class CompletedEventRepository: ICompletedEventRepository
    {
        private readonly UnitOfWork _unitOfWork;

        public CompletedEventRepository(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IEnumerable<CompletedEvent> Get(Expression<Func<CompletedEvent, bool>> criteria, params Expression<Func<CompletedEvent, object>>[] includeProperties)
        {
            var completedEventsSet = _unitOfWork.CompletedEvents as IQueryable<CompletedEvent>;

            completedEventsSet = includeProperties.Aggregate(completedEventsSet,
                (current, includeProperty) => current.Include(includeProperty));

            return completedEventsSet.Where(criteria).ToArray();
        }
    }
}
