using AutoMapper;
using CPMS.Domain;
using CPMS.Trust.Manager;
using CPMS.Trust.Presentation;

namespace CPMS.Trust.Delivery.Adapters
{
    public static class OOMappingConfig
    {
        public static void RegisterMappings()
        {
            // Domain Model to Application DTOs
            Mapper.CreateMap<Hospital, HospitalInfo>().ForSourceMember(src => src.Specialties, opt => opt.Ignore());
            Mapper.CreateMap<Specialty, SpecialtyInfo>()
                .ForSourceMember(src => src.Hospitals, opt => opt.Ignore())
                .ForSourceMember(src => src.Clinicians, opt => opt.Ignore());
            Mapper.CreateMap<Clinician, ClinicianInfo>()
                .ForSourceMember(src => src.Hospital, opt => opt.Ignore())
                .ForSourceMember(src => src.Specialty, opt => opt.Ignore());

            // Application DTOs to Presentation View Models
            Mapper.CreateMap<HospitalInfo, HospitalViewModel>();
            Mapper.CreateMap<SpecialtyInfo, SpecialtyViewModel>();
            Mapper.CreateMap<ClinicianInfo, ClinicianViewModel>();
        }
    }
}