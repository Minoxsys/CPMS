using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using CPMS.Patient.Domain;

namespace CPMS.Notification.Core.Adapters
{
    public class ErrorRepository : IErrorRepository
    {
        public IEnumerable<Error> Get(Expression<Func<Error, bool>> criteria)
        {
            using (var unitOfWork = new UnitOfWork())
            {
                return unitOfWork.Errors.Include(e => e.Period.Pathway.Patient).Where(criteria).ToArray();
            }
        }

        public void Save(Error error)
        {
            using (var unitOfWork = new UnitOfWork())
            {
                unitOfWork.Errors.Add(error);
            }
        }
    }
}
