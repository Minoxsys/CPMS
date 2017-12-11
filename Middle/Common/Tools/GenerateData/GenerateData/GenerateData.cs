using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using CPMS.Configuration;
using CPMS.Domain;

namespace GenerateData
{
    public partial class GenerateData : Form
    {
        private readonly Random _random = new Random();
        private readonly List<List<EventConnection>> _eventConnections = new List<List<EventConnection>>();
        private const string NoLongerInUse = "no longer in use";

        public GenerateData()
        {
            InitializeComponent();
        }

        private void GeneratePatients_Click(object sender, EventArgs e)
        {
            using (var unitOfWork = new UnitOfWork())
            {
                for (var patientIndex = 0; patientIndex < 5000; patientIndex++)
                {
                    var nhsNumber = GetUniqueIndex("123456", patientIndex);
                    var patient = new Patient
                    {
                        ConsultantName = "Consultant" + patientIndex,
                        ConsultantNumber = patientIndex.ToString(CultureInfo.InvariantCulture),
                        NHSNumber = nhsNumber,
                        Name = "Patient name" + patientIndex,
                        Title = "Mr",
                        DateOfBirth =
                            new DateTime(_random.Next(1910, 2010), _random.Next(1, 12), _random.Next(1, 28))
                    };
                    unitOfWork.Patients.Add(patient);
                    Console.WriteLine("Added patient {0} to UOW", nhsNumber);
                }
                Console.WriteLine("Persisting UOW to DB...");
                unitOfWork.SaveChanges();
                Console.WriteLine("Done adding patients!");
            }
        }

        private void GenerateClinicians_Click(object sender, EventArgs e)
        {
            using (var unitOfWork = new UnitOfWork())
            {
                var hospitals = unitOfWork.Hospitals.ToList();
                var specialties = unitOfWork.Specialties.ToList();

                for (var clinicianId = 0; clinicianId < 100; clinicianId++)
                {
                    var clinician = new Clinician
                    {
                        Hospital = hospitals[_random.Next(0, hospitals.Count - 1)],
                        Name = "Clinician name" + clinicianId,
                        Specialty = specialties[_random.Next(0, specialties.Count - 1)]
                    };
                    unitOfWork.Clinicians.Add(clinician);
                    Console.WriteLine("Added clinician {0} to UOW", clinician.Name);
                }

                Console.WriteLine("Persisting UOW to DB...");
                unitOfWork.SaveChanges();
                Console.WriteLine("Done adding clinicians!");
            }
        }

        private void GeneratePathways_Click(object sender, EventArgs e)
        {
            using (var unitOfWork = new UnitOfWork())
            {
                var patients = unitOfWork.Patients.ToList();

                var pathwayCount = string.IsNullOrEmpty(txtPathwayCount.Text) ? 3000 : Int32.Parse(txtPathwayCount.Text);

                for (var pathwayIndex = 0; pathwayIndex < pathwayCount; pathwayIndex++)
                {
                    var pathway = new Pathway
                    {
                        OrganizationCode = "org" + pathwayIndex,
                        Patient = patients[_random.Next(0, patients.Count - 1)],
                        PPINumber = GetUniqueIndex("123456", pathwayIndex)
                    };

                    unitOfWork.Pathways.Add(pathway);
                    Console.WriteLine("Added pathway {0} to UOW", pathway.PPINumber);

                }

                Console.WriteLine("Persisting UOW to DB...");
                unitOfWork.SaveChanges();
                Console.WriteLine("Done adding pathways!");
            }
        }

        private void GenerateCompletedEvents_Click(object sender, EventArgs e)
        {
            using (var unitOfWork = new UnitOfWork())
            {
                var sourceEvents = unitOfWork.SourceEvents
                .Include(s => s.NextPossibleEvents.Select(d => d.EventForDateReferenceForTarget))
                .Include(s => s.SourceName)
                .ToList();

                var possibleConnectionsList = (from sourceEvent in sourceEvents
                                               where sourceEvent.NextPossibleEvents != null
                                               from destinationEvent in sourceEvent.NextPossibleEvents
                                               select new EventConnection
                                               {
                                                   Parent = sourceEvent.SourceName.Code,
                                                   Child = destinationEvent.DestinationName.Code,
                                                   TargetNumberOfDays = destinationEvent.TargetNumberOfDays,
                                                   EventForDateReferenceForTarget = destinationEvent.EventForDateReferenceForTarget != null ? destinationEvent.EventForDateReferenceForTarget.Code : (EventCode?) null
                                               }).ToList();

                GenerateEventConnections(possibleConnectionsList, new List<EventConnection>(), EventCode.ReferralReceived);

                var pathways = unitOfWork.Pathways.ToList();
                var clinicians = unitOfWork.Clinicians.ToList();

                var pathwayIndex = 0;

                var periodCount = string.IsNullOrEmpty(txtPeriodCount.Text) ? _eventConnections.Count : Int32.Parse(txtPeriodCount.Text);

                var eventNames = unitOfWork.EventNames.ToList();

                for (var periodIndex = 0; periodIndex < periodCount; periodIndex++)
                {
                    var eventList = _eventConnections[periodIndex];

                    var period = new RTT18WeekPeriod
                    {
                        Pathway = pathways[pathwayIndex],
                        IsActive = true,
                        Name = "Period 1",
                        StartDate = new DateTime(DateTime.Now.Year, _random.Next(1, DateTime.Now.Month + 1), _random.Next(1, 28))
                    };

                    unitOfWork.Periods.Add(period);
                    Console.WriteLine("Added period {0} to UOW", period.Name);

                    pathwayIndex++;

                    for (var eventIndex = 0; eventIndex < eventList.Count; eventIndex++)
                    {
                        var eventCode = eventList[eventIndex].Parent;
                        var eventName = eventNames.FirstOrDefault(name => name.Code == (EventCode) eventCode);

                        var eventForReference = period.CompletedEvents.FirstOrDefault(completedEvent => completedEvent.Name.Code == (EventCode?)eventList[eventIndex].EventForDateReferenceForTarget);

                        if (eventIndex == 0)
                        {
                            var completedEvent = new ClockStartingCompletedEvent
                            {
                                Cancer = false,
                                Period = period,
                                Name = eventName,
                                Comments = string.Empty,
                                EventDate = period.StartDate,
                                Clinician = clinicians[_random.Next(0, clinicians.Count - 1)],
                                IsActive = eventIndex == eventList.Count - 1,
                                TargetDate = null
                            };

                            unitOfWork.CompletedEvents.Add(completedEvent);
                            Console.WriteLine("Added completed event {0} to UOW", completedEvent.Name.Description);
                            GenerateEventMilestonesForEvent(period, completedEvent, eventNames, unitOfWork);
                            Console.WriteLine("Added event milestones for event {0} to UOW", completedEvent.Name.Description);
                        }
                        else
                        {
                            var targetDate = (eventForReference != null && eventList[eventIndex].TargetNumberOfDays != null)
                                ? eventForReference.EventDate.AddDays((int)eventList[eventIndex].TargetNumberOfDays)
                                : (DateTime?)null;
                            var lastCompletedEvent = period.CompletedEvents.LastOrDefault();

                            var eventDateBasedOnTarget = (targetDate != null)
                                ? targetDate.Value.AddDays(_random.Next(-5, 5))
                                : (DateTime?)null;

                            var eventDate = (eventDateBasedOnTarget != null && lastCompletedEvent != null)
                                ? (eventDateBasedOnTarget >= lastCompletedEvent.EventDate
                                    ? (DateTime)eventDateBasedOnTarget
                                    : lastCompletedEvent.EventDate.AddDays(_random.Next(0, 5)))
                                : lastCompletedEvent.EventDate.AddDays(_random.Next(0, 5));

                            var completedEvent = new ClockTickingCompletedEvent
                            {
                                Cancer = false,
                                Period = period,
                                Name = eventName,
                                Comments = string.Empty,
                                EventDate = eventDate,
                                Clinician = clinicians[_random.Next(0, clinicians.Count - 1)],
                                IsActive = eventIndex == eventList.Count - 1,
                                TargetDate = targetDate
                            };
                            unitOfWork.CompletedEvents.Add(completedEvent);
                            Console.WriteLine("Added completed event {0} to UOW", completedEvent.Name.Description);

                            GenerateEventMilestonesForEvent(period, completedEvent, eventNames, unitOfWork);
                            Console.WriteLine("Added event milestones for event {0} to UOW", completedEvent.Name.Description);
                        }
                    }

                    Console.WriteLine("Persisting UOW to DB...");
                    unitOfWork.SaveChanges();
                    Console.WriteLine("Done Adding events!");
                }
            }
        }

        private void GenerateEventMilestonesForEvent(Period period, CompletedEvent currentCompletedEvent, List<EventName> eventNames, UnitOfWork unitOfWork)
        {
            var eventMilestoneDateReferenceForTargetCancerPolicy = new EventMilestoneDateReferenceForTargetCancerPolicy();
            var eventMilestoneTargetNumberOfDaysCancerPolicy = new EventMilestoneTargetNumberOfDaysCancerPolicy();

            var destinationEvents = GetDestinationEventsForSourceEventType(currentCompletedEvent.Name.Code, unitOfWork);

            foreach (var destinationEvent in destinationEvents)
            {
                var dateReferenceForTarget = ComputeDateReference(destinationEvent, currentCompletedEvent, period);

                var eventMilestone = new EventMilestone
                {
                    CompletedEvent = currentCompletedEvent,
                    IsMandatory = destinationEvent.IsMandatory,
                    TargetNumberOfDays = destinationEvent.TargetNumberOfDays,
                    Name = eventNames.FirstOrDefault(name => name.Code == destinationEvent.DestinationName.Code),
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
                            eventNames.FirstOrDefault(name => name.Code == destinationEvent.EventForDateReferenceForTarget.Code));
                    }

                    unitOfWork.EventMilestones.Add(eventMilestone);
                }
            }
        }

        private DateTime? ComputeDateReference(DestinationEvent destinationEvent, CompletedEvent currentCompletedEvent, Period period)
        {
            var destinationEventForDateReferenceForTarget = destinationEvent.EventForDateReferenceForTarget == null ? (EventCode?)null : destinationEvent.EventForDateReferenceForTarget.Code;

            var eventReference = period.CompletedEvents
                .OrderByDescending(e => e.EventDate)
                .FirstOrDefault(e => e.Period.Pathway.PPINumber == currentCompletedEvent.Period.Pathway.PPINumber && e.Name.Code == destinationEventForDateReferenceForTarget);

            if (eventReference != null)
            {
                return eventReference.EventDate;
            }
            return null;
        }

        private IEnumerable<DestinationEvent> GetDestinationEventsForSourceEventType(EventCode eventCode, UnitOfWork unitOfWork)
        {
            var sourceEvent = unitOfWork.SourceEvents.Include(e => e.NextPossibleEvents).FirstOrDefault(s => s.SourceName.Code == eventCode);
            if (sourceEvent != null)
            {
                var eventEventMilestones = sourceEvent.NextPossibleEvents.ToList();

                return eventEventMilestones;
            }

            return null;
        }

        private void GenerateEventConnections(IEnumerable<EventConnection> parentList, List<EventConnection> childList, EventCode parent)
        {
            var treeNodes = parentList as EventConnection[] ?? parentList.ToArray();

            foreach (var parentItem in treeNodes.Where(t => t.Parent == parent))
            {
                var period = new List<EventConnection>();
                period.AddRange(childList);
                period.Add(parentItem);
                _eventConnections.Add(period);

                if (parentItem.Child > parentItem.Parent)
                {
                    GenerateEventConnections(treeNodes, period, parentItem.Child);
                }
            }
            _eventConnections.Add(childList);
        }

        private string GetUniqueIndex(string index, int patientIndex)
        {
            return index + new string('0', 4 - patientIndex.ToString(CultureInfo.InvariantCulture).Length) + patientIndex;
        }

        private void GenerateSpecialtiesHospitals_Click(object sender, EventArgs e)
        {
            using (var unitOfWork = new UnitOfWork())
            {
                var hospital1 = new Hospital { Name = "Hospital 1" };
                var hospital2 = new Hospital { Name = "Hospital 2" };
                var hospital3 = new Hospital { Name = "Hospital 3" };
                var hospital4 = new Hospital { Name = "Hospital 4" };
                var hospital5 = new Hospital { Name = "Hospital 5" };

                var hospitals = new[]
                {
                    hospital1,
                    hospital2,
                    hospital3,
                    hospital4,
                    hospital5
                };

                var specialty1 = new Specialty { Code = "100", Name = "GENERAL SURGERY", Hospitals = hospitals };
                var specialty2 = new Specialty { Code = "101", Name = "UROLOGY", Hospitals = hospitals };
                var specialty3 = new Specialty { Code = "110", Name = "TRAUMA & ORTHOPAEDICS", Hospitals = hospitals };
                var specialty4 = new Specialty { Code = "120", Name = "ENT", Hospitals = hospitals };
                var specialty5 = new Specialty { Code = "130", Name = "OPHTHALMOLOGY", Hospitals = hospitals };
                var specialty6 = new Specialty { Code = "140", Name = "ORAL SURGERY", Hospitals = hospitals };
                var specialty7 = new Specialty { Code = "141", Name = "RESTORATIVE DENTISTRY", Hospitals = hospitals };
                var specialty8 = new Specialty { Code = "142", Name = "PAEDIATRIC DENTISTRY", Hospitals = hospitals };
                var specialty9 = new Specialty { Code = "143", Name = "ORTHODONTICS", Hospitals = hospitals };
                var specialty10 = new Specialty { Code = "145", Name = "ORAL & MAXILLO FACIAL SURGERY", Hospitals = hospitals };
                var specialty11 = new Specialty { Code = "146", Name = "ENDODONTICS", Hospitals = hospitals };
                var specialty12 = new Specialty { Code = "147", Name = "PERIODONTICS", Hospitals = hospitals };
                var specialty13 = new Specialty { Code = "148", Name = "PROSTHODONTICS", Hospitals = hospitals };
                var specialty14 = new Specialty { Code = "149", Name = "SURGICAL DENTISTRY", Hospitals = hospitals };
                var specialty15 = new Specialty { Code = "150", Name = "NEUROSURGERY", Hospitals = hospitals };
                var specialty16 = new Specialty { Code = "160", Name = "PLASTIC SURGERY", Hospitals = hospitals };
                var specialty17 = new Specialty { Code = "170", Name = "CARDIOTHORACIC SURGERY", Hospitals = hospitals };
                var specialty18 = new Specialty { Code = "171", Name = "PAEDIATRIC SURGERY", Hospitals = hospitals };
                var specialty19 = new Specialty { Code = "180", Name = "ACCIDENT & EMERGENCY", Hospitals = hospitals };
                var specialty20 = new Specialty { Code = "190", Name = "ANAESTHETICS", Hospitals = hospitals };
                var specialty21 = new Specialty { Code = "191", Name = NoLongerInUse, Hospitals = hospitals };
                var specialty22 = new Specialty { Code = "192", Name = "CRITICAL CARE MEDICINE", Hospitals = hospitals };
                var specialty23 = new Specialty { Code = "199", Name = "Non-UK provider; specialty function not known, treatment mainly surgical", Hospitals = hospitals };
                var specialty24 = new Specialty { Code = "300", Name = "GENERAL MEDICINE", Hospitals = hospitals };
                var specialty25 = new Specialty { Code = "301", Name = "GASTROENTEROLOGY", Hospitals = hospitals };
                var specialty26 = new Specialty { Code = "302", Name = "ENDOCRINOLOGY", Hospitals = hospitals };
                var specialty27 = new Specialty { Code = "303", Name = "CLINICAL HAEMATOLOGY", Hospitals = hospitals };
                var specialty28 = new Specialty { Code = "304", Name = "CLINICAL PHYSIOLOGY", Hospitals = hospitals };
                var specialty29 = new Specialty { Code = "305", Name = "CLINICAL PHARMACOLOGY", Hospitals = hospitals };
                var specialty30 = new Specialty { Code = "310", Name = "AUDIOLOGICAL MEDICINE", Hospitals = hospitals };
                var specialty31 = new Specialty { Code = "311", Name = "CLINICAL GENETICS", Hospitals = hospitals };
                var specialty32 = new Specialty { Code = "312", Name = "CLINICAL CYTOGENETICS and MOLECULAR GENETICS", Hospitals = hospitals };
                var specialty33 = new Specialty { Code = "313", Name = "CLINICAL IMMUNOLOGY and ALLERGY", Hospitals = hospitals };
                var specialty34 = new Specialty { Code = "314", Name = "REHABILITATION", Hospitals = hospitals };
                var specialty35 = new Specialty { Code = "315", Name = "PALLIATIVE MEDICINE", Hospitals = hospitals };
                var specialty36 = new Specialty { Code = "320", Name = "CARDIOLOGY", Hospitals = hospitals };
                var specialty37 = new Specialty { Code = "321", Name = "PAEDIATRIC CARDIOLOGY", Hospitals = hospitals };
                var specialty38 = new Specialty { Code = "325", Name = "SPORTS AND EXERCISE MEDICINE ", Hospitals = hospitals };
                var specialty39 = new Specialty { Code = "326", Name = "ACUTE INTERNAL MEDICINE", Hospitals = hospitals };
                var specialty40 = new Specialty { Code = "330", Name = "DERMATOLOGY", Hospitals = hospitals };
                var specialty41 = new Specialty { Code = "340", Name = "RESPIRATORY MEDICINE (also known as thoracic medicine)", Hospitals = hospitals };
                var specialty42 = new Specialty { Code = "350", Name = "INFECTIOUS DISEASES", Hospitals = hospitals };
                var specialty43 = new Specialty { Code = "352", Name = "TROPICAL MEDICINE", Hospitals = hospitals };
                var specialty44 = new Specialty { Code = "360", Name = "GENITOURINARY MEDICINE", Hospitals = hospitals };
                var specialty45 = new Specialty { Code = "361", Name = "NEPHROLOGY", Hospitals = hospitals };
                var specialty46 = new Specialty { Code = "370", Name = "MEDICAL ONCOLOGY", Hospitals = hospitals };
                var specialty47 = new Specialty { Code = "371", Name = "NUCLEAR MEDICINE", Hospitals = hospitals };
                var specialty48 = new Specialty { Code = "400", Name = "NEUROLOGY", Hospitals = hospitals };
                var specialty49 = new Specialty { Code = "401", Name = "CLINICAL NEURO-PHYSIOLOGY", Hospitals = hospitals };
                var specialty50 = new Specialty { Code = "410", Name = "RHEUMATOLOGY", Hospitals = hospitals };
                var specialty51 = new Specialty { Code = "420", Name = "PAEDIATRICS", Hospitals = hospitals };
                var specialty52 = new Specialty { Code = "421", Name = "PAEDIATRIC NEUROLOGY", Hospitals = hospitals };
                var specialty53 = new Specialty { Code = "430", Name = "GERIATRIC MEDICINE", Hospitals = hospitals };
                var specialty54 = new Specialty { Code = "450", Name = "DENTAL MEDICINE SPECIALTIES", Hospitals = hospitals };
                var specialty55 = new Specialty { Code = "451", Name = "SPECIAL CARE DENTISTRY", Hospitals = hospitals };
                var specialty56 = new Specialty { Code = "460", Name = "MEDICAL OPHTHALMOLOGY", Hospitals = hospitals };
                var specialty57 = new Specialty { Code = "499", Name = "Non-UK provider; specialty function not known, treatment mainly medical", Hospitals = hospitals };
                var specialty58 = new Specialty { Code = "500", Name = "OBSTETRICS and GYNAECOLOGY", Hospitals = hospitals };
                var specialty59 = new Specialty { Code = "501", Name = "OBSTETRICS", Hospitals = hospitals };
                var specialty60 = new Specialty { Code = "502", Name = "GYNAECOLOGY", Hospitals = hospitals };
                var specialty61 = new Specialty { Code = "504", Name = "COMMUNITY SEXUAL AND REPRODUCTIVE HEALTH", Hospitals = hospitals };
                var specialty62 = new Specialty { Code = "510", Name = NoLongerInUse, Hospitals = hospitals };
                var specialty63 = new Specialty { Code = "520", Name = NoLongerInUse, Hospitals = hospitals };
                var specialty64 = new Specialty { Code = "560", Name = "MIDWIFE EPISODE", Hospitals = hospitals };
                var specialty65 = new Specialty { Code = "600", Name = "GENERAL MEDICAL PRACTICE", Hospitals = hospitals };
                var specialty66 = new Specialty { Code = "601", Name = "GENERAL DENTAL PRACTICE", Hospitals = hospitals };
                var specialty67 = new Specialty { Code = "610", Name = NoLongerInUse, Hospitals = hospitals };
                var specialty68 = new Specialty { Code = "620", Name = NoLongerInUse, Hospitals = hospitals };
                var specialty69 = new Specialty { Code = "700", Name = "LEARNING DISABILITY", Hospitals = hospitals };
                var specialty70 = new Specialty { Code = "710", Name = "ADULT MENTAL ILLNESS", Hospitals = hospitals };
                var specialty71 = new Specialty { Code = "711", Name = "CHILD and ADOLESCENT PSYCHIATRY", Hospitals = hospitals };
                var specialty72 = new Specialty { Code = "712", Name = "FORENSIC PSYCHIATRY", Hospitals = hospitals };
                var specialty73 = new Specialty { Code = "713", Name = "PSYCHOTHERAPY", Hospitals = hospitals };
                var specialty74 = new Specialty { Code = "715", Name = "OLD AGE PSYCHIATRY", Hospitals = hospitals };
                var specialty75 = new Specialty { Code = "800", Name = "CLINICAL ONCOLOGY (previously RADIOTHERAPY)", Hospitals = hospitals };
                var specialty76 = new Specialty { Code = "810", Name = "RADIOLOGY", Hospitals = hospitals };
                var specialty77 = new Specialty { Code = "820", Name = "GENERAL PATHOLOGY", Hospitals = hospitals };
                var specialty78 = new Specialty { Code = "821", Name = "BLOOD TRANSFUSION", Hospitals = hospitals };
                var specialty79 = new Specialty { Code = "822", Name = "CHEMICAL PATHOLOGY", Hospitals = hospitals };
                var specialty80 = new Specialty { Code = "823", Name = "HAEMATOLOGY", Hospitals = hospitals };
                var specialty81 = new Specialty { Code = "824", Name = "HISTOPATHOLOGY", Hospitals = hospitals };
                var specialty82 = new Specialty { Code = "830", Name = "IMMUNOPATHOLOGY", Hospitals = hospitals };
                var specialty83 = new Specialty { Code = "831", Name = "MEDICAL MICROBIOLOGY AND VIROLOGY", Hospitals = hospitals };
                var specialty84 = new Specialty { Code = "832", Name = NoLongerInUse, Hospitals = hospitals };
                var specialty85 = new Specialty { Code = "833", Name = "MEDICAL MICROBIOLOGY (also known as Microbiology and Bacteriology)", Hospitals = hospitals };
                var specialty86 = new Specialty { Code = "834", Name = "MEDICAL VIROLOGY", Hospitals = hospitals };
                var specialty87 = new Specialty { Code = "900", Name = "COMMUNITY MEDICINE", Hospitals = hospitals };
                var specialty88 = new Specialty { Code = "901", Name = "OCCUPATIONAL MEDICINE", Hospitals = hospitals };
                var specialty89 = new Specialty { Code = "902", Name = "COMMUNITY HEALTH SERVICES DENTAL", Hospitals = hospitals };
                var specialty90 = new Specialty { Code = "903", Name = "PUBLIC HEALTH MEDICINE", Hospitals = hospitals };
                var specialty91 = new Specialty { Code = "904", Name = "PUBLIC HEALTH DENTAL", Hospitals = hospitals };
                var specialty92 = new Specialty { Code = "950", Name = "NURSING EPISODE", Hospitals = hospitals };
                var specialty93 = new Specialty { Code = "960", Name = "ALLIED HEALTH PROFESSIONAL EPISODE", Hospitals = hospitals };
                var specialty94 = new Specialty { Code = "990", Name = NoLongerInUse, Hospitals = hospitals };

                hospital1.Specialties = new[] { specialty1, specialty2, specialty3, specialty4, specialty5, specialty6, specialty7, specialty8, specialty9, specialty10, specialty11, specialty12, specialty13, specialty14, specialty15, specialty16, specialty17, specialty18, specialty19, specialty20, specialty21, specialty22, specialty23, specialty24, specialty25, specialty26, specialty27, specialty28, specialty29, specialty30, specialty31, specialty32, specialty33, specialty34, specialty35, specialty36, specialty37, specialty38, specialty39, specialty40, specialty41, specialty42, specialty43, specialty44, specialty45, specialty46, specialty47, specialty48, specialty49, specialty50, specialty51, specialty52, specialty53, specialty54, specialty55, specialty56, specialty57, specialty58, specialty59, specialty60, specialty61, specialty62, specialty63, specialty64, specialty65, specialty66, specialty67, specialty68, specialty69, specialty70, specialty71, specialty72, specialty73, specialty74, specialty75, specialty76, specialty77, specialty78, specialty79, specialty80, specialty81, specialty82, specialty83, specialty84, specialty85, specialty86, specialty87, specialty88, specialty89, specialty90, specialty91, specialty92, specialty93, specialty94 };
                hospital2.Specialties = new[] { specialty1, specialty2, specialty3, specialty4, specialty5, specialty6, specialty7, specialty8, specialty9, specialty10, specialty11, specialty12, specialty13, specialty14, specialty15, specialty16, specialty17, specialty18, specialty19, specialty20, specialty21, specialty22, specialty23, specialty24, specialty25, specialty26, specialty27, specialty28, specialty29, specialty30, specialty31, specialty32, specialty33, specialty34, specialty35, specialty36, specialty37, specialty38, specialty39, specialty40, specialty41, specialty42, specialty43, specialty44, specialty45, specialty46, specialty47, specialty48, specialty49, specialty50, specialty51, specialty52, specialty53, specialty54, specialty55, specialty56, specialty57, specialty58, specialty59, specialty60, specialty61, specialty62, specialty63, specialty64, specialty65, specialty66, specialty67, specialty68, specialty69, specialty70, specialty71, specialty72, specialty73, specialty74, specialty75, specialty76, specialty77, specialty78, specialty79, specialty80, specialty81, specialty82, specialty83, specialty84, specialty85, specialty86, specialty87, specialty88, specialty89, specialty90, specialty91, specialty92, specialty93, specialty94 };
                hospital3.Specialties = new[] { specialty1, specialty2, specialty3, specialty4, specialty5, specialty6, specialty7, specialty8, specialty9, specialty10, specialty11, specialty12, specialty13, specialty14, specialty15, specialty16, specialty17, specialty18, specialty19, specialty20, specialty21, specialty22, specialty23, specialty24, specialty25, specialty26, specialty27, specialty28, specialty29, specialty30, specialty31, specialty32, specialty33, specialty34, specialty35, specialty36, specialty37, specialty38, specialty39, specialty40, specialty41, specialty42, specialty43, specialty44, specialty45, specialty46, specialty47, specialty48, specialty49, specialty50, specialty51, specialty52, specialty53, specialty54, specialty55, specialty56, specialty57, specialty58, specialty59, specialty60, specialty61, specialty62, specialty63, specialty64, specialty65, specialty66, specialty67, specialty68, specialty69, specialty70, specialty71, specialty72, specialty73, specialty74, specialty75, specialty76, specialty77, specialty78, specialty79, specialty80, specialty81, specialty82, specialty83, specialty84, specialty85, specialty86, specialty87, specialty88, specialty89, specialty90, specialty91, specialty92, specialty93, specialty94 };
                hospital4.Specialties = new[] { specialty1, specialty2, specialty3, specialty4, specialty5, specialty6, specialty7, specialty8, specialty9, specialty10, specialty11, specialty12, specialty13, specialty14, specialty15, specialty16, specialty17, specialty18, specialty19, specialty20, specialty21, specialty22, specialty23, specialty24, specialty25, specialty26, specialty27, specialty28, specialty29, specialty30, specialty31, specialty32, specialty33, specialty34, specialty35, specialty36, specialty37, specialty38, specialty39, specialty40, specialty41, specialty42, specialty43, specialty44, specialty45, specialty46, specialty47, specialty48, specialty49, specialty50, specialty51, specialty52, specialty53, specialty54, specialty55, specialty56, specialty57, specialty58, specialty59, specialty60, specialty61, specialty62, specialty63, specialty64, specialty65, specialty66, specialty67, specialty68, specialty69, specialty70, specialty71, specialty72, specialty73, specialty74, specialty75, specialty76, specialty77, specialty78, specialty79, specialty80, specialty81, specialty82, specialty83, specialty84, specialty85, specialty86, specialty87, specialty88, specialty89, specialty90, specialty91, specialty92, specialty93, specialty94 };
                hospital5.Specialties = new[] { specialty1, specialty2, specialty3, specialty4, specialty5, specialty6, specialty7, specialty8, specialty9, specialty10, specialty11, specialty12, specialty13, specialty14, specialty15, specialty16, specialty17, specialty18, specialty19, specialty20, specialty21, specialty22, specialty23, specialty24, specialty25, specialty26, specialty27, specialty28, specialty29, specialty30, specialty31, specialty32, specialty33, specialty34, specialty35, specialty36, specialty37, specialty38, specialty39, specialty40, specialty41, specialty42, specialty43, specialty44, specialty45, specialty46, specialty47, specialty48, specialty49, specialty50, specialty51, specialty52, specialty53, specialty54, specialty55, specialty56, specialty57, specialty58, specialty59, specialty60, specialty61, specialty62, specialty63, specialty64, specialty65, specialty66, specialty67, specialty68, specialty69, specialty70, specialty71, specialty72, specialty73, specialty74, specialty75, specialty76, specialty77, specialty78, specialty79, specialty80, specialty81, specialty82, specialty83, specialty84, specialty85, specialty86, specialty87, specialty88, specialty89, specialty90, specialty91, specialty92, specialty93, specialty94 };

                Console.WriteLine("Persisting UOW to DB...");
                var specialties = new List<Specialty> {specialty1, specialty2, specialty3, specialty4, specialty5, specialty6, specialty7, specialty8, specialty9, specialty10, specialty11, specialty12, specialty13, specialty14, specialty15, specialty16, specialty17, specialty18, specialty19, specialty20, specialty21, specialty22, specialty23, specialty24, specialty25, specialty26, specialty27, specialty28, specialty29, specialty30, specialty31, specialty32, specialty33, specialty34, specialty35, specialty36, specialty37, specialty38, specialty39, specialty40, specialty41, specialty42, specialty43, specialty44, specialty45, specialty46, specialty47, specialty48, specialty49, specialty50, specialty51, specialty52, specialty53, specialty54, specialty55, specialty56, specialty57, specialty58, specialty59, specialty60, specialty61, specialty62, specialty63, specialty64, specialty65, specialty66, specialty67, specialty68, specialty69, specialty70, specialty71, specialty72, specialty73, specialty74, specialty75, specialty76, specialty77, specialty78, specialty79, specialty80, specialty81, specialty82, specialty83, specialty84, specialty85, specialty86, specialty87, specialty88, specialty89, specialty90, specialty91, specialty92, specialty93, specialty94};
                foreach (var specialty in specialties)
                {
                    var existingSpecialty = unitOfWork.Specialties.FirstOrDefault(s => s.Code == specialty.Code);
                    if (existingSpecialty != null)
                    {
                        unitOfWork.Specialties.Remove(existingSpecialty);
                    }
                    unitOfWork.Specialties.Add(specialty);
                }

                unitOfWork.SaveChanges();
                Console.WriteLine("Done adding hospitals and specialties!");
            }
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool AllocConsole();

        private void GenerateData_Load(object sender, EventArgs e)
        {
            AllocConsole();
        }
    }
}
