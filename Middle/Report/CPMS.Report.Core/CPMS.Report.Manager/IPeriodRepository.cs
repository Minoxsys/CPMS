﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using CPMS.Domain;

namespace CPMS.Report.Manager
{
    public interface IPeriodRepository
    {
        IEnumerable<Period> Get(Expression<Func<Period, bool>> criteria, DateTime? fromDate = null, params Expression<Func<Period, object>>[] includeProperties);

        int Count(Expression<Func<Period, bool>> criteria);
    }
}