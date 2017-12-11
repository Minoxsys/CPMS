﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using CPMS.Patient.Domain;

namespace CPMS.Report.Core.Adapters
{
    public class EventRepository: IEventRepository
    {
        public IEnumerable<Event> Get(Expression<Func<Event, bool>> criteria, ListInput infoModel)
        {
            using (var unitOfWork = new UnitOfWork())
            {
                return GetEventsByCriteria(criteria, unitOfWork).ToArray();
            }
        }

        public int Count(Expression<Func<Event, bool>> criteria)
        {
            using (var unitOfWork = new UnitOfWork())
            {
                return GetEventsByCriteria(criteria, unitOfWork).Count();
            }
        }

        private IEnumerable<Event> GetEventsByCriteria(Expression<Func<Event, bool>> criteria, UnitOfWork unitOfWork)
        {
            return unitOfWork.Events
                .Include(e => e.Period)
                .Include(e => e.Clinician.Specialty)
                .Include(e => e.Clinician.Hospital)
                .Where(criteria.Compile());
        }

    }
}