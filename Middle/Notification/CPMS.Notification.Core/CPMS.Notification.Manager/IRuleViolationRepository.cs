using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using CPMS.Domain;

namespace CPMS.Notification.Manager
{
    public interface IRuleViolationRepository
    {
        IEnumerable<RuleViolation> Get(Expression<Func<RuleViolation, bool>> criteria, params Expression<Func<RuleViolation, object>>[] includeProperties);
    }
}
