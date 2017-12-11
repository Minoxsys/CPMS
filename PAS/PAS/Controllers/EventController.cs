using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using CPMS.Configuration;
using CPMS.Domain;
using PAS.Models;
using ClockType = PAS.Models.ClockType;

namespace PAS.Controllers
{
    public class EventController : Controller
    {
        private const string PeriodName = "Period ";

        [HttpGet]
        public ActionResult Add()
        {
            var addCompletedEventViewModel = new AddCompletedEventViewModel();
            InitializeViewModel(addCompletedEventViewModel);

            return View(addCompletedEventViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(AddCompletedEventInputModel addCompletedEventInputModel)
        {
            if (ModelState.IsValid)
            {
                using (var unitOfWork = new UnitOfWork())
                {
                    var currentPeriod = GetCurrentActivePeriod(addCompletedEventInputModel.SelectedPPINumber, unitOfWork) ??
                                        GetLastPeriod(addCompletedEventInputModel.SelectedPPINumber, unitOfWork);

                    Period period;
                    if (addCompletedEventInputModel.SelectedEventCode == EventCode.ReferralReceived)
                    {
                        var pathway =
                            unitOfWork.Pathways.Include(p => p.Patient).Include(p=>p.Periods).FirstOrDefault(p => p.PPINumber == addCompletedEventInputModel.SelectedPPINumber);

                        if (addCompletedEventInputModel.Cancer)
                        {
                            period = new CancerPeriod
                            {
                                IsActive = true,
                                Pathway = pathway,
                                Name = GetNextPeriodName(currentPeriod),
                                StartDate = addCompletedEventInputModel.Date.Value
                            };
                        }
                        else
                        {
                            period = new RTT18WeekPeriod
                            {
                                IsActive = true,
                                Pathway = pathway,
                                Name = GetNextPeriodName(currentPeriod),
                                StartDate = addCompletedEventInputModel.Date.Value
                            };
                        }

                        SetCurrentPeriodToInactive(currentPeriod);
                        SetStopDateForNon18WPeriod(currentPeriod, addCompletedEventInputModel.Date.Value);

                        AddPeriodToPathway(pathway, period, unitOfWork);
                    }
                    else
                    {
                        period = currentPeriod;

                        if (period == null || !period.IsActive)
                        {
                            if (period == null || period.StopDate != null)
                            {
                                var periodName = (period != null && !string.IsNullOrEmpty(period.Name))
                                    ? GetNextPeriodName(period)
                                    : string.Concat(PeriodName, "1");

                                var pathway =
                                    unitOfWork.Pathways.FirstOrDefault(
                                        p => p.PPINumber == addCompletedEventInputModel.SelectedPPINumber);

                                period = new Non18WeekPeriod
                                {
                                    IsActive = true,
                                    Pathway = pathway,
                                    Name = periodName,
                                    StartDate = addCompletedEventInputModel.Date.Value
                                };

                                AddPeriodToPathway(pathway, period, unitOfWork);
                            }
                        }
                        else
                        {
                            if (addCompletedEventInputModel.Cancer && !(period.GetType() == typeof(CancerPeriod)))
                            {
                                var cancerPeriod = new CancerPeriod
                                {
                                    IsActive = period.IsActive,
                                    Name = period.Name,
                                    Pathway = period.Pathway,
                                    StartDate = period.StartDate,
                                    StopDate = period.StopDate
                                };

                                AddPeriodToPathway(period.Pathway, cancerPeriod, unitOfWork);

                                unitOfWork.Periods.Add(cancerPeriod);
                                unitOfWork.SaveChanges();

                                foreach (var completedEvent in period.CompletedEvents)
                                {
                                    completedEvent.Period = cancerPeriod;
                                    cancerPeriod.Add(completedEvent);
                                }

                                unitOfWork.Periods.Remove(period);
                                unitOfWork.SaveChanges();

                                period = cancerPeriod;
                            }
                        }
                    }

                    var eventNames = unitOfWork.EventNames.ToList();
                    var newCompletedEvent = BuildCompletedEvent(addCompletedEventInputModel.SelectedClockType, addCompletedEventInputModel.SelectedEventCode, addCompletedEventInputModel.Date.Value, period);

                    newCompletedEvent.Comments = addCompletedEventInputModel.Comments;
                    newCompletedEvent.EventDate = addCompletedEventInputModel.Date.Value;
                    newCompletedEvent.Clinician = GetClinician(addCompletedEventInputModel.SelectedClinician, unitOfWork);
                    newCompletedEvent.Name = eventNames.FirstOrDefault(eventName => eventName.Code == addCompletedEventInputModel.SelectedEventCode);
                    newCompletedEvent.Cancer = addCompletedEventInputModel.Cancer;
                    newCompletedEvent.IsActive = true;
                    if (newCompletedEvent.GetType() == typeof(ClockStoppingCompletedEvent))
                    {
                        newCompletedEvent.IsActive = false;
                        period.IsActive = false;
                        period.StopDate = newCompletedEvent.EventDate;
                    }
                    period.ValidationFailed += ruleViolation =>
                    {
                        ruleViolation.CreatedAt = DateTime.UtcNow;
                        unitOfWork.RuleViolations.Add(ruleViolation);
                    };
                    period.Add(newCompletedEvent);

                    newCompletedEvent.Period = period;

                    newCompletedEvent.TargetDate = GetTargetReferenceDate(newCompletedEvent, unitOfWork);
                    new CompletedEventTargetDateCancerPolicy().ApplyTo(newCompletedEvent);

                    newCompletedEvent.IsBreached = newCompletedEvent.PostBreachDays != null;

                    SetCurrentCompletedEventToInactive(addCompletedEventInputModel.SelectedPPINumber, unitOfWork);

                    newCompletedEvent.ValidationFailed += ruleViolation =>
                    {
                        ruleViolation.CreatedAt = DateTime.UtcNow;
                        unitOfWork.RuleViolations.Add(ruleViolation);
                    };
                    newCompletedEvent.Validate();

                    unitOfWork.CompletedEvents.Add(newCompletedEvent);
                    unitOfWork.SaveChanges();

                    if (newCompletedEvent.Period.GetType() != typeof(Non18WeekPeriod))
                    {
                        AddEventMilestonesForCompletedEvent(period, newCompletedEvent, eventNames, unitOfWork);
                        unitOfWork.SaveChanges();
                    }
                }

                return RedirectToAction("Add");
            }

            InitializeViewModel(addCompletedEventInputModel);

            return View(addCompletedEventInputModel);
        }

        private void SetStopDateForNon18WPeriod(Period currentPeriod, DateTime dateTime)
        {
            if (currentPeriod != null && currentPeriod.GetType() == typeof(Non18WeekPeriod))
            {
                currentPeriod.StopDate = dateTime;
            }
        }

        private void AddPeriodToPathway(Pathway pathway, Period period, UnitOfWork unitOfWork)
        {
            pathway.ValidationFailed += ruleViolation =>
            {
                ruleViolation.CreatedAt = DateTime.UtcNow;
                unitOfWork.RuleViolations.Add(ruleViolation);
            };
            pathway.Add(period);
        }

        private string GetNextPeriodName(Period currentPeriod)
        {
            var currentPeriodNumber = 0;
            if (currentPeriod != null)
            {
                currentPeriodNumber = Int32.Parse(currentPeriod.Name.Split(' ')[1]);
            }

            return PeriodName + (++currentPeriodNumber);
        }

        private void InitializeViewModel(AddCompletedEventViewModel addCompletedEventViewModel)
        {
            addCompletedEventViewModel.AllCompletedEvents = GetAllCompletedEvents();
            addCompletedEventViewModel.Clinicians = GetAllClinicians();
            addCompletedEventViewModel.Pathways = GetPathways();
            addCompletedEventViewModel.EventNames = GetEventNames();
        }

        private IEnumerable<EventNameViewModel> GetEventNames()
        {
            using (var unitOfWork = new UnitOfWork())
            {
                return unitOfWork.EventNames.Select(eventName => new EventNameViewModel { EventCode = eventName.Code, Description = eventName.Description}).ToList();
            }
        }

        private IEnumerable<CompletedEventViewModel> GetAllCompletedEvents()
        {
            using (var unitOfWork = new UnitOfWork())
            {
                var completedEvents = unitOfWork.CompletedEvents
                    .Include(p => p.Clinician)
                    .Include(p => p.Period.Pathway)
                    .Include(p => p.Name)
                    .ToArray();

                return completedEvents.Select(
                    completedEvent =>
                        new CompletedEventViewModel
                        {
                            Id = completedEvent.Id,
                            Description = completedEvent.Name.Description,
                            Date = completedEvent.EventDate.ToShortDateString(),
                            Clinician = completedEvent.Clinician.Name,
                            PPINumber = completedEvent.Period.Pathway.PPINumber,
                            Comments = completedEvent.Comments,
                            Cancer = completedEvent.Cancer
                        });
            }
        }

        private Clinician GetClinician(int clinicianId, UnitOfWork unitOfWork)
        {
            return unitOfWork.Clinicians.FirstOrDefault(clinician => clinician.Id == clinicianId);
        }

        private IEnumerable<LiteClinicianViewModel> GetAllClinicians()
        {
            using (var unitOfWork = new UnitOfWork())
            {
                return
                    unitOfWork.Clinicians.ToArray().Select(
                        clinician =>
                            new LiteClinicianViewModel
                            {
                                Name = clinician.Name,
                                Id = clinician.Id
                            });
            }
        }

        private IEnumerable<string> GetPathways()
        {
            using (var unitOfWork = new UnitOfWork())
            {
                var pathways = unitOfWork.Pathways
                    .Include(p => p.Patient)
                    .Select(p => p.PPINumber)
                    .ToList();

                return pathways;
            }
        }

        private void AddEventMilestonesForCompletedEvent(Period period, CompletedEvent currentCompletedEvent, List<EventName> eventNames, UnitOfWork unitOfWork)
        {
            var eventMilestoneDateReferenceForTargetCancerPolicy = new EventMilestoneDateReferenceForTargetCancerPolicy();
            var eventMilestoneTargetNumberOfDaysCancerPolicy = new EventMilestoneTargetNumberOfDaysCancerPolicy();

            var destinationEvents = GetDestinationEventsForSourceEventType(currentCompletedEvent.Name.Code, unitOfWork);

            foreach (var destinationEvent in destinationEvents)
            {
                var dateReferenceForTarget = ComputeDateReference(destinationEvent, currentCompletedEvent, unitOfWork);

                var eventMilestone = new EventMilestone
                {
                    CompletedEvent = currentCompletedEvent,
                    IsMandatory = destinationEvent.IsMandatory,
                    TargetNumberOfDays = destinationEvent.TargetNumberOfDays,
                    Name = eventNames.FirstOrDefault(eventName => eventName.Code == (EventCode)destinationEvent.DestinationName.Code),
                    DateReferenceForTarget = dateReferenceForTarget
                };

                eventMilestoneDateReferenceForTargetCancerPolicy.ApplyTo(eventMilestone);
                eventMilestoneTargetNumberOfDaysCancerPolicy.ApplyTo(eventMilestone);

                if (((currentCompletedEvent.Name.Code == EventCode.PatientCancelEvent || currentCompletedEvent.Name.Code == EventCode.HospitalCancelEvent || currentCompletedEvent.Name.Code == EventCode.DidNotAttend) && (period.CompletedEvents.Any(e => e.Name.Code == destinationEvent.EventForDateReferenceForTarget.Code))) ||
                    (currentCompletedEvent.Name.Code != EventCode.PatientCancelEvent && currentCompletedEvent.Name.Code != EventCode.HospitalCancelEvent && currentCompletedEvent.Name.Code != EventCode.DidNotAttend))
                {
                    if (destinationEvent.EventForDateReferenceForTarget != null)
                    {
                        period.MapEventMilestoneToCompletedEvent(currentCompletedEvent, eventMilestone,
                            eventNames.FirstOrDefault(eventName => eventName.Code == destinationEvent.EventForDateReferenceForTarget.Code));
                    }

                    unitOfWork.EventMilestones.Add(eventMilestone);
                }
            }
        }

        private DateTime? ComputeDateReference(DestinationEvent destinationEvent, CompletedEvent currentCompletedEvent, UnitOfWork unitOfWork)
        {
            var destinationEventForDateReferenceForTarget = destinationEvent.EventForDateReferenceForTarget == null ? (EventCode?)null : destinationEvent.EventForDateReferenceForTarget.Code;

            var eventReference = unitOfWork.CompletedEvents
                .Include(e => e.Period.Pathway)
                .OrderByDescending(e => e.EventDate)
                .FirstOrDefault(e => e.Period.Pathway.PPINumber == currentCompletedEvent.Period.Pathway.PPINumber && e.Name.Code == destinationEventForDateReferenceForTarget);

            if (eventReference != null)
            {
                return eventReference.EventDate;
            }
            return null;
        }

        private DateTime? GetTargetReferenceDate(CompletedEvent currentCompletedEvent, UnitOfWork unitOfWork)
        {
            var destinationEvent = unitOfWork.DestinationEvents.FirstOrDefault(e => e.DestinationName.Code == currentCompletedEvent.Name.Code);

            if (destinationEvent == null || destinationEvent.TargetNumberOfDays == null || destinationEvent.EventForDateReferenceForTarget == null) return null;

            var referenceEventForTargetDate = currentCompletedEvent.Period.CompletedEvents.OrderBy(ev => ev.EventDate).ToList().LastOrDefault(ev => ev.Name.Code == destinationEvent.EventForDateReferenceForTarget.Code);

            if (referenceEventForTargetDate == null)
            {
                return null;
            }
            return referenceEventForTargetDate.EventDate.AddDays((int)destinationEvent.TargetNumberOfDays).AddDays(currentCompletedEvent.Period.GetNumberOfPausedDays(currentCompletedEvent.EventDate, referenceEventForTargetDate.EventDate));
        }

        private IEnumerable<DestinationEvent> GetDestinationEventsForSourceEventType(EventCode eventCode, UnitOfWork unitOfWork)
        {
            var sourceEvent = unitOfWork.SourceEvents.Include(e => e.NextPossibleEvents).FirstOrDefault(s => s.SourceName.Code == eventCode);
            if (sourceEvent != null)
            {
                var eventMilestones = sourceEvent.NextPossibleEvents.ToList();

                return eventMilestones;
            }

            return null;
        }

        private Period GetCurrentActivePeriod(string ppiNumber, UnitOfWork unitOfWork)
        {
            return unitOfWork.Periods
                .Include(p => p.Pathway.Patient)
                .Include(p => p.CompletedEvents)
                .FirstOrDefault(e => e.Pathway.PPINumber == ppiNumber && e.IsActive);
        }

        private Period GetLastPeriod(string ppiNumber, UnitOfWork unitOfWork)
        {
            return unitOfWork.Periods
                .Include(p => p.Pathway)
                .Include(p => p.CompletedEvents)
                .OrderByDescending(p => p.Id)
                .FirstOrDefault(e => e.Pathway.PPINumber == ppiNumber);
        }

        private void SetCurrentPeriodToInactive(Period currentActivePeriod)
        {
            if (currentActivePeriod != null)
            {
                currentActivePeriod.IsActive = false;
            }
        }

        private void SetCurrentCompletedEventToInactive(string ppiNumber, UnitOfWork unitOfWork)
        {
            var currentActiveCompletedEvent = unitOfWork.CompletedEvents
                .Include(p => p.Period.Pathway)
                .FirstOrDefault(e => e.Period.Pathway.PPINumber == ppiNumber && e.IsActive);

            if (currentActiveCompletedEvent != null)
            {
                currentActiveCompletedEvent.IsActive = false;
            }
        }

        private CompletedEvent BuildCompletedEvent(ClockType clockType, EventCode eventCode, DateTime eventDate, Period period)
        {
            CompletedEvent newCompletedEvent;

            if (eventCode == EventCode.PatientCancelEvent && period.IsSecondCancelByPatient(EventCode.PatientCancelEvent))
            {
                newCompletedEvent = new ClockStoppingCompletedEvent();
            }
            else
            {
                if (eventCode == EventCode.HospitalCancelEvent)
                {
                    newCompletedEvent = new ClockTickingCompletedEvent();
                }
                else
                {
                    if (eventCode == EventCode.DidNotAttend &&
                        (period.IsSecondDidNotAttend(eventCode) && period.Pathway.Patient.IsChild(eventDate) ||
                        period.IsSecondDidNotAttend(eventCode) && period.AbleToNotAttendFirstEvent ||
                        !(period.Pathway.Patient.IsChild(eventDate) || period.AbleToNotAttendFirstEvent)))
                    {
                        newCompletedEvent = new ClockStoppingCompletedEvent();
                    }
                    else
                    {
                        switch (clockType)
                        {
                            case ClockType.ClockStarting:
                                {
                                    newCompletedEvent = new ClockStartingCompletedEvent();
                                    break;
                                }
                            case ClockType.ClockTicking:
                                {
                                    newCompletedEvent = new ClockTickingCompletedEvent();
                                    break;
                                }
                            case ClockType.ClockPausing:
                                {
                                    newCompletedEvent = new ClockPausingCompletedEvent();
                                    break;
                                }
                            case ClockType.ClockStopping:
                                {
                                    newCompletedEvent = new ClockStoppingCompletedEvent();
                                    break;
                                }
                            default:
                                {
                                    newCompletedEvent = new ClockTickingCompletedEvent();
                                    break;
                                }
                        }
                    }
                }
            }
            return newCompletedEvent;
        }
    }
}