using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using CPMS.Domain;
using IClinicianRepository = CPMS.Report.Manager.IClinicianRepository;

namespace CPMS.Report.Core.Adapters
{
    public class ClinicianRepository : IClinicianRepository
    {
        private readonly UnitOfWork _unitOfWork;

        public ClinicianRepository(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IEnumerable<Clinician> Get(Expression<Func<Clinician, bool>> criteria, params Expression<Func<Clinician, object>>[] includeProperties)
        {
            var cliniciansSet = _unitOfWork.Clinicians as IQueryable<Clinician>;

            cliniciansSet = includeProperties.Aggregate(cliniciansSet,
                (current, includeProperty) => current.Include(includeProperty));

            return cliniciansSet.Where(criteria).ToArray();
        }
    }
}
