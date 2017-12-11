using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using CPMS.Patient.Domain;

namespace CPMS.Core.Adapters
{
    public class HospitalRepository : IHospitalRepository
    {
        public IEnumerable<Hospital> Get(Expression<Func<Hospital, bool>> criteria)
        {
            using (var unitOfWork = new UnitOfWork())
            {
                return unitOfWork.Hospitals.Where(criteria).ToArray();
            }
        }
    }
}
