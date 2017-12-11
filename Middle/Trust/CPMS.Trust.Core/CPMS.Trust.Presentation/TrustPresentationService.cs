using System.Collections.Generic;
using System.Linq;
using CPMS.Authorization;
using CPMS.Trust.Manager;

namespace CPMS.Trust.Presentation
{
    public class TrustPresentationService
    {
        private readonly TrustAuthorizationFilter _trustAuthorizationFilter;
        private readonly IMapper<HospitalInfo, HospitalViewModel> _hospitalInfoToHospitalViewModelMapper;
        private readonly IMapper<SpecialtyInfo, SpecialtyViewModel> _specialtyInfoToSpecialtyViewModelMapper;
        private readonly IMapper<ClinicianInfo, ClinicianViewModel> _clinicianInfoToClinicianViewModelMapper;

        public TrustPresentationService(
            TrustAuthorizationFilter trustAuthorizationFilter,
            IMapper<HospitalInfo, HospitalViewModel> hospitalInfoToHospitalViewModelMapper,
            IMapper<SpecialtyInfo, SpecialtyViewModel> specialtyInfoToSpecialtyViewModelMapper,
            IMapper<ClinicianInfo, ClinicianViewModel> clinicianInfoToClinicianViewModelMapper)
        {
            _trustAuthorizationFilter = trustAuthorizationFilter;
            _hospitalInfoToHospitalViewModelMapper = hospitalInfoToHospitalViewModelMapper;
            _specialtyInfoToSpecialtyViewModelMapper = specialtyInfoToSpecialtyViewModelMapper;
            _clinicianInfoToClinicianViewModelMapper = clinicianInfoToClinicianViewModelMapper;
        }

        public virtual IEnumerable<HospitalViewModel> GetHospitals(RoleData role)
        {
            return _trustAuthorizationFilter.GetHospitals(role).Select(hospital => _hospitalInfoToHospitalViewModelMapper.Map(hospital));
        }

        public virtual IEnumerable<SpecialtyViewModel> GetSpecialties(RoleData role, int? hospitalId)
        {
            return _trustAuthorizationFilter.GetSpecialties(role, hospitalId).Select(specialty => _specialtyInfoToSpecialtyViewModelMapper.Map(specialty)).ToArray();
        }

        public virtual IEnumerable<ClinicianViewModel> GetClinicians(RoleData role, int? hospitalId, string specialtyCode)
        {
            return _trustAuthorizationFilter.GetClinicians(role, hospitalId, specialtyCode).Select(clinician => _clinicianInfoToClinicianViewModelMapper.Map(clinician));
        }
    }
}
