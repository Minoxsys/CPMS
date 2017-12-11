using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using CPMS.Domain;
using IHospitalRepository = CPMS.Trust.Manager.IHospitalRepository;

namespace CPMS.Trust.Core.Adapters
{
    public class HospitalRepository : IHospitalRepository
    {
        private readonly UnitOfWork _unitOfWork;

        public HospitalRepository(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IEnumerable<Hospital> Get(Expression<Func<Hospital, bool>> criteria, params Expression<Func<Hospital, object>>[] includeProperties)
        {
            var hospitalsSet = _unitOfWork.Hospitals as IQueryable<Hospital>;

            hospitalsSet = includeProperties.Aggregate(hospitalsSet,
                (current, includeProperty) => current.Include(includeProperty));

            return hospitalsSet.Where(criteria).ToArray();
        }
    }
}
