using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using CPMS.Patient.Domain;
using PAS.Models;

namespace PAS.Controllers
{
    public class PatientController : Controller
    {
        public ActionResult Add()
        {
            var viewModel = new AddPatientViewModel
            {
                AllPatients = GetAllPatients()
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(AddPatientViewModel addPatientInputModel)
        {
            if (ModelState.IsValid)
            {
                var patient = new Patient
                {
                    Title = addPatientInputModel.Title.ToString(),
                    Name = addPatientInputModel.Name,
                    DateOfBirth = addPatientInputModel.DateOfBirth.Value,
                    NHSNumber = addPatientInputModel.NHSNumber,
                    ConsultantName = addPatientInputModel.ConsultantName,
                    ConsultantNumber = addPatientInputModel.ConsultantNumber
                };
                using (var unitOfwork = new UnitOfWork())
                {
                    patient.ValidationFailed += error =>
                    {
                        error.CreatedAt = DateTime.UtcNow;
                        unitOfwork.Errors.Add(error);
                    };

                    patient.Validate();

                    unitOfwork.Patients.Add(patient);
                    unitOfwork.SaveChanges();
                }

                return RedirectToAction("Add");
            }

            addPatientInputModel.AllPatients = GetAllPatients();

            return View(addPatientInputModel);
        }

        private IEnumerable<PatientViewModel> GetAllPatients()
        {
            using (var unitOfWork = new UnitOfWork())
            {
                return
                    unitOfWork.Patients.ToArray().Select(
                        patient =>
                            new PatientViewModel
                            {
                                Title = patient.Title,
                                Name = patient.Name,
                                DateOfBirth = patient.DateOfBirth.ToShortDateString(),
                                NHSNumber = patient.NHSNumber,
                                ConsultantName = patient.ConsultantName,
                                ConsultantNumber = patient.ConsultantNumber
                            });
            }
        }
    }
}
