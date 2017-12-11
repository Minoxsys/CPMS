using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using CPMS.Domain;
using PAS.Models;

namespace PAS.Controllers
{
    public class PathwayController : Controller
    {
        [HttpGet]
        public ActionResult Add()
        {
            var addPathwayViewModel = new AddPathwayViewModel
            {
                Patients = GetAllPatients(),
                AllPathways = GetAllPathways()
            };

            return View(addPathwayViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(AddPathwayInputModel pathwayInputModel)
        {
            if (ModelState.IsValid)
            {
                using (var unitOfwork = new UnitOfWork())
                {
                    var pathway = new Pathway
                    {
                        PPINumber = pathwayInputModel.PPINumber,
                        Patient = GetPatientByNhsNumber(pathwayInputModel.SelectedPatientNHSNumber, unitOfwork),
                        OrganizationCode = pathwayInputModel.OrganizationCode
                    };

                    pathway.ValidationFailed += ruleViolation =>
                    {
                        ruleViolation.CreatedAt = DateTime.UtcNow;
                        unitOfwork.RuleViolations.Add(ruleViolation);
                    };
                    pathway.Validate();

                    unitOfwork.Pathways.Add(pathway);
                    unitOfwork.SaveChanges();
                }

                return RedirectToAction("Add");
            }

            pathwayInputModel.Patients = GetAllPatients();
            pathwayInputModel.AllPathways = GetAllPathways();

            return View(pathwayInputModel);
        }

        private Patient GetPatientByNhsNumber(string nhsNumber, UnitOfWork unitOfWork)
        {
            return
                unitOfWork.Patients.FirstOrDefault(patient => patient.NHSNumber == nhsNumber);
        }

        private IEnumerable<LitePatientViewModel> GetAllPatients()
        {
            using (var unitOfWork = new UnitOfWork())
            {
                return
                    unitOfWork.Patients.ToArray().Select(
                        patient =>
                            new LitePatientViewModel
                            {
                                Name = patient.Name,
                                NHSNumber = patient.NHSNumber
                            });
            }
        }

        private IEnumerable<PathwayViewModel> GetAllPathways()
        {
            using (var unitOfWork = new UnitOfWork())
            {
                return unitOfWork.Pathways
                    .Include(p => p.Patient)
                    .ToArray()
                    .Select(
                        pathway =>
                            new PathwayViewModel
                            {
                                NHSNumber = pathway.Patient.NHSNumber,
                                PPINumber = pathway.PPINumber,
                                OrganizationCode = pathway.OrganizationCode
                            });
            }
        }
    }
}