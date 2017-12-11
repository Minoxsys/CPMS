using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace CPMS.Patient.Domain
{
    public interface IErrorRepository
    {
        IEnumerable<Error> Get(Expression<Func<Error, bool>> criteria);

        void Save(Error error);
    }
}
