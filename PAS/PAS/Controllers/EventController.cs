using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using CPMS.Configuration;
using CPMS.Patient.Domain;
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
            var eventViewModel = new AddEventViewModel();
            InitializeViewModel(eventViewModel);

            return View(eventViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(AddEventInputModel eventInputModel)
        {
            if (ModelState.IsValid)
            {
                using (var unitOfWork = new UnitOfWork())
                {
                    var currentPeriod = GetCurrentActivePeriod(eventInputModel.SelectedPPINumber, unitOfWork) ??
                                        GetLastPeriod(eventInputModel.SelectedPPINumber, unitOfWork);

                    Period period;
                    if (eventInputModel.SelectedEventCode == EventCode.ReferralReceived)
                    {
                        var pathway =
                            unitOfWork.Pathways.Include(p => p.Patient).Include(p=>p.Periods).FirstOrDefault(p => p.PPINumber == eventInputModel.SelectedPPINumber);

                        if (eventInputModel.Cancer)
                        {
                            period = new CancerPeriod
                            {
                                IsActive = true,
                                Pathway = pathway,
                                Name = GetNextPeriodName(currentPeriod),
                                StartDate = eventInputModel.Date.Value
                            };
                        }
                        else
                        {
                            period = new RTT18WeekPeriod
                            {
                                IsActive = true,
                                Pathway = pathway,
                                Name = GetNextPeriodName(currentPeriod),
                                StartDate = eventInputModel.Date.Value
                            };
                        }

                        SetCurrentPeriodToInactive(currentPeriod);
                        SetStopDateForNon18WPeriod(currentPeriod, eventInputModel.Date.Value);

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
                                        p => p.PPINumber == eventInputModel.SelectedPPINumber);

                                period = new Non18WeekPeriod
                                {
                                    IsActive = true,
                                    Pathway = pathway,
                                    Name = periodName,
                                    StartDate = eventInputModel.Date.Value
                                };

                                AddPeriodToPathway(pathway, period, unitOfWork);
                            }
                        }
                        else
                        {
                            if (eventInputModel.Cancer && !(period.GetType() == typeof(CancerPeriod)))
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

                                foreach (var @event in period.Events)
                                {
                                    @event.Period = cancerPeriod;
                                    cancerPeriod.Add(@event);
                                }

                                unitOfWork.Periods.Remove(period);
                                unitOfWork.SaveChanges();

                                period = cancerPeriod;
                            }
                        }
                    }

                    var newEvent = BuildEvent(eventInputModel.SelectedClockType, eventInputModel.SelectedEventCode, eventInputModel.Date.Value, period);

                    newEvent.Comments = eventInputModel.Comments;
                    newEvent.EventDate = eventInputModel.Date.Value;
                    newEvent.Clinician = GetClinician(eventInputModel.SelectedClinician, unitOfWork);
                    newEvent.Code = eventInputModel.SelectedEventCode;
                    newEvent.Cancer = eventInputModel.Cancer;
                    newEvent.IsActive = true;
                    if (newEvent.GetType() == typeof(ClockStoppingEvent))
                    {
                        newEvent.IsActive = false;
                        period.IsActive = false;
                        period.StopDate = newEvent.EventDate;
                    }
                    period.ValidationFailed += error =>
                    {
                        error.CreatedAt = DateTime.UtcNow;
                        unitOfWork.Errors.Add(error);
                    };
                    period.Add(newEvent);

                    newEvent.Period = period;

                    newEvent.TargetDate = GetTargetReferenceDate(newEvent, unitOfWork);
                    new EventTargetDateCancerPolicy().ApplyTo(newEvent);

                    SetCurrentEventToInactive(eventInputModel.SelectedPPINumber, unitOfWork);

                    newEvent.ValidationFailed += error =>
                    {
                        error.CreatedAt = DateTime.UtcNow;
                        unitOfWork.Errors.Add(error);
                    };
                    newEvent.Validate();

                    unitOfWork.Events.Add(newEvent);
                    unitOfWork.SaveChanges();

                    if (newEvent.Period.GetType() != typeof(Non18WeekPeriod))
                    {
                        AddPlannedEventsForEvent(period, newEvent, unitOfWork);
                        unitOfWork.SaveChanges();
                    }
                }

                return RedirectToAction("Add");
            }


            InitializeViewModel(eventInputModel);

            return View(eventInputModel);
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
            pathway.ValidationFailed += error =>
            {
                error.CreatedAt = DateTime.UtcNow;
                unitOfWork.Errors.Add(error);
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

        private void InitializeViewModel(AddEventViewModel eventInputModel)
        {
            eventInputModel.AllEvents = GetAllEvents();
            eventInputModel.Clinicians = GetAllClinicians();
            eventInputModel.Pathways = GetPathways();
        }

        private IEnumerable<EventViewModel> GetAllEvents()
        {
            using (var unitOfWork = new UnitOfWork())
            {
                var events = unitOfWork.Events
                    .Include(p => p.Clinician)
                    .Include(p => p.Period.Pathway)
                    .ToArray();

                return events.Select(
                    ev =>
                        new EventViewModel
                        {
                            Id = ev.Id,
                            Code = ev.Code,
                            Date = ev.EventDate.ToShortDateString(),
                            Clinician = ev.Clinician.Name,
                            PPINumber = ev.Period.Pathway.PPINumber,
                            Comments = ev.Comments,
                            Cancer = ev.Cancer
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

        private void AddPlannedEventsForEvent(Period period, Event currentEvent, UnitOfWork unitOfWork)
        {
            var plannedEventDateReferenceForTargetCancerPolicy = new PlannedEventDateReferenceForTargetCancerPolicy();
            var plannedEventTargetNumberOfDaysCancerPolicy = new PlannedEventTargetNumberOfDaysCancerPolicy();

            var destinationEvents = GetDestinationEventsForSourceEventType(currentEvent.Code, unitOfWork);

            foreach (var destinationEvent in destinationEvents)
            {
                var dateReferenceForTarget = ComputeDateReference(destinationEvent, currentEvent, unitOfWork);

                var plannedEvent = new PlannedEvent
                {
                    Event = currentEvent,
                    IsMandatory = destinationEvent.IsMandatory,
                    TargetNumberOfDays = destinationEvent.TargetNumberOfDays,
                    Code = (EventCode)destinationEvent.DestinationCode,
                    DateReferenceForTarget = dateReferenceForTarget
                };

                plannedEventDateReferenceForTargetCancerPolicy.ApplyTo(plannedEvent);
                plannedEventTargetNumberOfDaysCancerPolicy.ApplyTo(plannedEvent);

                if (((currentEvent.Code == EventCode.PatientCancelEvent || currentEvent.Code == EventCode.HospitalCancelEvent || currentEvent.Code == EventCode.DidNotAttend) && (period.Events.Any(e => e.Code == (CPMS.Patient.Domain.EventCode)destinationEvent.EventForDateReferenceForTarget))) ||
                    (currentEvent.Code != EventCode.PatientCancelEvent && currentEvent.Code != EventCode.HospitalCancelEvent && currentEvent.Code != EventCode.DidNotAttend))
                {
                    if (destinationEvent.EventForDateReferenceForTarget != null)
                    {
                        period.MapPlannedEventToEvent(currentEvent, plannedEvent,
                            (EventCode)destinationEvent.EventForDateReferenceForTarget);
                    }

                    unitOfWork.PlannedEvents.Add(plannedEvent);
                }
            }
        }

        private DateTime? ComputeDateReference(DestinationEvent destinationEvent, Event currentEvent, UnitOfWork unitOfWork)
        {
            var destinationEventForDateReferenceForTarget = destinationEvent.EventForDateReferenceForTarget == null ? (EventCode?)null : (EventCode)destinationEvent.EventForDateReferenceForTarget;

            var eventReference = unitOfWork.Events
                .Include(e => e.Period.Pathway)
                .OrderByDescending(e => e.EventDate)
                .FirstOrDefault(e => e.Period.Pathway.PPINumber == currentEvent.Period.Pathway.PPINumber && e.Code == destinationEventForDateReferenceForTarget);

            if (eventReference != null)
            {
                return eventReference.EventDate;
            }
            return null;
        }

        private DateTime? GetTargetReferenceDate(Event currentEvent, UnitOfWork unitOfWork)
        {
            var destinationEvent = unitOfWork.DestinationEvents.FirstOrDefault(e => e.DestinationCode == (ConfigurationEventCode)currentEvent.Code);

            if (destinationEvent == null || destinationEvent.TargetNumberOfDays == null || destinationEvent.EventForDateReferenceForTarget == null) return null;

            var referenceEventForTargetDate = currentEvent.Period.Events.OrderBy(ev => ev.EventDate).ToList().LastOrDefault(ev => ev.Code == (EventCode)destinationEvent.EventForDateReferenceForTarget);

            if (referenceEventForTargetDate == null)
            {
                return null;
            }
            return referenceEventForTargetDate.EventDate.AddDays((int)destinationEvent.TargetNumberOfDays).AddDays(currentEvent.Period.GetNumberOfPausedDays(currentEvent.EventDate, referenceEventForTargetDate.EventDate));
        }

        private IEnumerable<DestinationEvent> GetDestinationEventsForSourceEventType(EventCode eventCode, UnitOfWork unitOfWork)
        {
            var sourceEvent = unitOfWork.SourceEvents.Include(e => e.NextPossibleEvents).FirstOrDefault(s => s.SourceCode == (ConfigurationEventCode)eventCode);
            if (sourceEvent != null)
            {
                var plannedEvents = sourceEvent.NextPossibleEvents.ToList();

                return plannedEvents;
            }

            return null;
        }

        private Period GetCurrentActivePeriod(string ppiNumber, UnitOfWork unitOfWork)
        {
            return unitOfWork.Periods
                .Include(p => p.Pathway.Patient)
                .Include(p => p.Events)
                .FirstOrDefault(e => e.Pathway.PPINumber == ppiNumber && e.IsActive);
        }

        private Period GetLastPeriod(string ppiNumber, UnitOfWork unitOfWork)
        {
            return unitOfWork.Periods
                .Include(p => p.Pathway)
                .Include(p => p.Events)
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

        private void SetCurrentEventToInactive(string ppiNumber, UnitOfWork unitOfWork)
        {
            var currentActiveEvent = unitOfWork.Events
                .Include(p => p.Period.Pathway)
                .FirstOrDefault(e => e.Period.Pathway.PPINumber == ppiNumber && e.IsActive);

            if (currentActiveEvent != null)
            {
                currentActiveEvent.IsActive = false;
            }
        }

        private Event BuildEvent(ClockType clockType, EventCode eventCode, DateTime eventDate, Period period)
        {
            Event newEvent;

            if (eventCode == EventCode.PatientCancelEvent && period.IsSecondCancelByPatient(EventCode.PatientCancelEvent))
            {
                newEvent = new ClockStoppingEvent();
            }
            else
            {
                if (eventCode == EventCode.HospitalCancelEvent)
                {
                    newEvent = new ClockTickingEvent();
                }
                else
                {
                    if (eventCode == EventCode.DidNotAttend &&
                        (period.IsSecondDidNotAttend(eventCode) && period.Pathway.Patient.IsChild(eventDate) ||
                        period.IsSecondDidNotAttend(eventCode) && period.AbleToNotAttendFirstEvent ||
                        !(period.Pathway.Patient.IsChild(eventDate) || period.AbleToNotAttendFirstEvent)))
                    {
                        newEvent = new ClockStoppingEvent();
                    }
                    else
                    {
                        switch (clockType)
                        {
                            case ClockType.ClockStarting:
                                {
                                    newEvent = new ClockStartingEvent();
                                    break;
                                }
                            case ClockType.ClockTicking:
                                {
                                    newEvent = new ClockTickingEvent();
                                    break;
                                }
                            case ClockType.ClockPausing:
                                {
                                    newEvent = new ClockPausingEvent();
                                    break;
                                }
                            case ClockType.ClockStopping:
                                {
                                    newEvent = new ClockStoppingEvent();
                                    break;
                                }
                            default:
                                {
                                    newEvent = new ClockTickingEvent();
                                    break;
                                }
                        }
                    }
                }
            }
            return newEvent;
        }
    }
}