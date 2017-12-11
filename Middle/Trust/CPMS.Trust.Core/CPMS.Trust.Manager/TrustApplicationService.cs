using System.Collections.Generic;
using System.Linq;
using CPMS.Domain;

namespace CPMS.Trust.Manager
{
    public class TrustApplicationService
    {
        private readonly IHospitalRepository _hospitalRepository;
        private readonly ISpecialtyRepository _specialtyRepository;
        private readonly IClinicianRepository _clinicianRepository;
        private readonly IMapper<Hospital, HospitalInfo> _hospitalToHospitalInfoMapper;
        private readonly IMapper<Specialty, SpecialtyInfo> _specialtyToSpecialtyInfoMapper;
        private readonly IMapper<Clinician, ClinicianInfo> _clinicianToClinicianInfoMapper;

        public TrustApplicationService(IHospitalRepository hospitalRepository,
            ISpecialtyRepository specialtyRepository,
            IClinicianRepository clinicianRepository,
            IMapper<Hospital, HospitalInfo> hospitalToHospitalInfoMapper,
            IMapper<Specialty, SpecialtyInfo> specialtyToSpecialtyInfoMapper,
            IMapper<Clinician, ClinicianInfo> clinicianToClinicianInfoMapper)
        {
            _hospitalRepository = hospitalRepository;
            _specialtyRepository = specialtyRepository;
            _clinicianRepository = clinicianRepository;
            _hospitalToHospitalInfoMapper = hospitalToHospitalInfoMapper;
            _specialtyToSpecialtyInfoMapper = specialtyToSpecialtyInfoMapper;
            _clinicianToClinicianInfoMapper = clinicianToClinicianInfoMapper;
        }

        public virtual IEnumerable<HospitalInfo> GetHospitals()
        {
            return _hospitalRepository.Get(hospital => true).Select(hospital => _hospitalToHospitalInfoMapper.Map(hospital));
        }

        public virtual IEnumerable<SpecialtyInfo> GetSpecialties(int? hospitalId)
        {
            return
                _specialtyRepository.Get(
                    specialty =>
                        hospitalId == null ||
                        (specialty.Hospitals.Any(hospital => hospital.Id == hospitalId) &&
                         specialty.Clinicians.Any(clinician => clinician.Hospital.Id == hospitalId)))
                    .Select(specialty => _specialtyToSpecialtyInfoMapper.Map(specialty));
        }

        public virtual IEnumerable<ClinicianInfo> GetClinicians(int? hospitalId, string specialtyCode)
        {
            return _clinicianRepository.Get(clinician =>
                    (string.IsNullOrEmpty(specialtyCode) || clinician.Specialty.Code == specialtyCode) &&
                    (hospitalId == null || clinician.Hospital.Id == hospitalId)).Select(clinician => _clinicianToClinicianInfoMapper.Map(clinician));
        }
    }
}
