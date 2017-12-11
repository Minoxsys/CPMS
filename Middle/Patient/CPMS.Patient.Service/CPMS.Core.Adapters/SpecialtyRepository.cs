using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using CPMS.Patient.Domain;

namespace CPMS.Core.Adapters
{
    public class SpecialtyRepository : ISpecialtyRepository
    {
        public IEnumerable<Specialty> Get(Expression<Func<Specialty, bool>> criteria)
        {
            using (var unitOfWork = new UnitOfWork())
            {
                return unitOfWork.Specialties.Where(criteria).ToArray();
            }
        }
    }
}
