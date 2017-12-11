using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using CPMS.Patient.Domain;
using PAS.Models;

namespace PAS.Controllers
{
    public class ClinicianController : Controller
    {
        [HttpGet]
        public ActionResult Add()
        {
            var addClinicianViewModel = new AddClinicianViewModel
            {
                Hospitals = GetHospitals(),
                Specialties = GetSpecialties(),
                AllClinicians = GetAllClinicians()
            };

            return View(addClinicianViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(AddClinicianInputModel clinicianInputModel)
        {
            if (ModelState.IsValid)
            {
                using (var unitOfwork = new UnitOfWork())
                {
                    var hospital = GetHospitalById(clinicianInputModel.SelectedHospital, unitOfwork);
                    var specialty = GetSpecialtyByCode(clinicianInputModel.SelectedSpecialty, unitOfwork);

                    var clinician = new Clinician
                    {
                        Name = clinicianInputModel.Name,
                        Hospital = hospital,
                        Specialty = specialty
                    };

                    unitOfwork.Clinicians.Add(clinician);
                    unitOfwork.SaveChanges();
                }

                return RedirectToAction("Add");
            }

            clinicianInputModel.Hospitals = GetHospitals();
            clinicianInputModel.Specialties = GetSpecialties();
            clinicianInputModel.AllClinicians = GetAllClinicians();

            return View(clinicianInputModel);
        }

        [HttpGet]
        public JsonResult GetSpecialtiesBy(int? hospitalId = null)
        {
            var specialties = new List<LiteSpecialtyViewModel> { new LiteSpecialtyViewModel {Code = "0", Name = "Select Specialty"} };
            specialties.AddRange(GetSpecialties(hospitalId));

            return new JsonResult {Data = specialties, JsonRequestBehavior = JsonRequestBehavior.AllowGet};
        }

        private IEnumerable<LiteHospitalViewModel> GetHospitals()
        {
            using (var unitOfWork = new UnitOfWork())
            {
                return unitOfWork.Hospitals.ToArray().Select(hospital => new LiteHospitalViewModel
                {
                    Id = hospital.Id,
                    Name = hospital.Name
                });
            }
        }

        private IEnumerable<LiteSpecialtyViewModel> GetSpecialties(int? hospitalId = null)
        {
            using (var unitOfWork = new UnitOfWork())
            {
                return hospitalId == null
                    ? unitOfWork.Specialties.ToArray().Select(GetLiteSpecialty)
                    : GetHospitalById((int) hospitalId, unitOfWork).Specialties.ToArray().Select(GetLiteSpecialty);
            }
        }

        private LiteSpecialtyViewModel GetLiteSpecialty(Specialty specialty)
        {
            return new LiteSpecialtyViewModel
            {
                Code = specialty.Code,
                Name = specialty.Name
            };
        }

        private IEnumerable<ClinicianViewModel> GetAllClinicians()
        {
            using (var unitOfWork = new UnitOfWork())
            {
                return unitOfWork.Clinicians
                    .Include(p => p.Hospital)
                    .Include(p => p.Specialty)
                    .ToArray().Select(clinician => new ClinicianViewModel
                                                    {
                                                        Hospital = clinician.Hospital.Name,
                                                        Specialty = clinician.Specialty.Name,
                                                        Name = clinician.Name
                                                    });
            }
        }

        private Specialty GetSpecialtyByCode(string code, UnitOfWork unitOfwork)
        {
            return unitOfwork.Specialties.FirstOrDefault(specialty => specialty.Code == code);
        }

        private Hospital GetHospitalById(int hospitalId, UnitOfWork unitOfwork)
        {
            return unitOfwork.Hospitals
                .Include(p => p.Specialties)
                .FirstOrDefault(hospital => hospital.Id == hospitalId);
        }
    }
}