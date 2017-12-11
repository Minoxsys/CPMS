using System.Collections.Generic;
using CPMS.Authorization;

namespace CPMS.Trust.Manager
{
    public class TrustAuthorizationFilter
    {
        private readonly TrustApplicationService _trustApplicationService;

        public TrustAuthorizationFilter(TrustApplicationService trustApplicationService)
        {
            _trustApplicationService = trustApplicationService;
        }

        public virtual IEnumerable<HospitalInfo> GetHospitals(RoleData roleData)
        {
            return _trustApplicationService.GetHospitals();
        }

        public virtual IEnumerable<SpecialtyInfo> GetSpecialties(RoleData roleData, int? hospitalId)
        {
            return _trustApplicationService.GetSpecialties(hospitalId);
        }

        public virtual IEnumerable<ClinicianInfo> GetClinicians(RoleData roleData, int? hospitalId, string specialtyCode)
        {
            return _trustApplicationService.GetClinicians(hospitalId, specialtyCode);
        }
    }
}
