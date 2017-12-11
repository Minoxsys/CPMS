using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using CPMS.Patient.Domain;

namespace CPMS.Core.Adapters
{
    public class ClinicianRepository : IClinicianRepository
    {
        public IEnumerable<Clinician> Get(Expression<Func<Clinician, bool>> criteria)
        {
            using (var unitOfWork = new UnitOfWork())
            {
                return unitOfWork.Clinicians.Include(c=>c.Hospital.Specialties).Where(criteria.Compile()).ToArray();
            }
        }
    }
}
