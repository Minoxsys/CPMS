using System.Collections.Generic;
using System.Linq;
using CPMS.Domain;
using CPMS.Report.Manager;

namespace CPMS.Report.Core.Adapters
{
    public class HospitalRepository : IHospitalRepository
    {
        private readonly UnitOfWork _unitOfWork;

        public HospitalRepository(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IEnumerable<Hospital> Get()
        {
            return _unitOfWork.Hospitals.ToArray();
        }
    }
}
