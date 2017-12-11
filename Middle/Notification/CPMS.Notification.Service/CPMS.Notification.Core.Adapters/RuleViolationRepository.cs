using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using CPMS.Domain;
using IRuleViolationRepository = CPMS.Notification.Manager.IRuleViolationRepository;

namespace CPMS.Notification.Core.Adapters
{
    public class RuleViolationRepository : IRuleViolationRepository
    {
        private readonly UnitOfWork _unitOfWork;

        public RuleViolationRepository(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IEnumerable<RuleViolation> Get(Expression<Func<RuleViolation, bool>> criteria, params Expression<Func<RuleViolation, object>>[] includeProperties)
        {
            var ruleViolationsSet = _unitOfWork.RuleViolations as IQueryable<RuleViolation>;

            ruleViolationsSet = includeProperties.Aggregate(ruleViolationsSet,
                (current, includeProperty) => current.Include(includeProperty));

            return ruleViolationsSet.Where(criteria).ToArray();
        }
    }
}
