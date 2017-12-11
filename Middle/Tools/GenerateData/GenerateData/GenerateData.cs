using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using CPMS.Configuration;
using CPMS.Patient.Domain;

namespace GenerateData
{
    public partial class GenerateData : Form
    {
        private readonly Random _random = new Random();
        private readonly List<List<EventConnection>> _eventConnections = new List<List<EventConnection>>();

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
                    {
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
                        Console.WriteLine("Added patient {0} to UOW", patient.Name);
                    }
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

                for (var pathwayIndex = 0; pathwayIndex < 3000; pathwayIndex++)
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

        private void GenerateEvents_Click(object sender, EventArgs e)
        {
            using (var unitOfWork = new UnitOfWork())
            {
                var sourceEvents = unitOfWork.SourceEvents
                .Include(s => s.NextPossibleEvents)
                .ToList();

                var possibleConnectionsList = (from sourceEvent in sourceEvents
                                               where sourceEvent.NextPossibleEvents != null
                                               from destinationEvent in sourceEvent.NextPossibleEvents
                                               select new EventConnection
                                               {
                                                   Parent = sourceEvent.SourceCode,
                                                   Child = destinationEvent.DestinationCode,
                                                   TargetNumberOfDays = destinationEvent.TargetNumberOfDays,
                                                   EventForDateReferenceForTarget = destinationEvent.EventForDateReferenceForTarget
                                               }).ToList();

                GenerateEventConnections(possibleConnectionsList, new List<EventConnection>(),
                    ConfigurationEventCode.ReferralReceived);

                var pathways = unitOfWork.Pathways.ToList();
                var clinicians = unitOfWork.Clinicians.ToList();

                var pathwayIndex = 0;

                for (var periodIndex = 0; periodIndex < _eventConnections.Count; periodIndex++)
                {
                    var eventList = _eventConnections[periodIndex];

                    var period = new RTT18WeekPeriod
                    {
                        Pathway = pathways[pathwayIndex],
                        IsActive = true,
                        Name = "Period" + periodIndex,
                        StartDate = new DateTime(2014, _random.Next(1, 9), _random.Next(1, 28))
                    };

                    unitOfWork.Periods.Add(period);
                    Console.WriteLine("Added period {0} to UOW", period.Name);

                    pathwayIndex++;

                    for (var eventIndex = 0; eventIndex < eventList.Count; eventIndex++)
                    {
                        var eventCode = eventList[eventIndex].Parent;

                        var eventForReference = period.Events.FirstOrDefault(ev => ev.Code == (EventCode?)eventList[eventIndex].EventForDateReferenceForTarget);

                        if (eventIndex == 0)
                        {
                            var evnt = new ClockStartingEvent
                            {
                                Cancer = false,
                                Period = period,
                                Code = (EventCode) eventCode,
                                Comments = string.Empty,
                                EventDate = period.StartDate,
                                Clinician = clinicians[_random.Next(0, clinicians.Count - 1)],
                                IsActive = eventIndex == eventList.Count - 1,
                                TargetDate = null
                            };

                            unitOfWork.Events.Add(evnt);
                            Console.WriteLine("Added event {0} to UOW", evnt.Code);
                        }
                        else
                        {
                            var targetDate = (eventForReference != null && eventList[eventIndex].TargetNumberOfDays != null)
                                ? eventForReference.EventDate.AddDays((int)eventList[eventIndex].TargetNumberOfDays)
                                : (DateTime?)null;
                            var lastEvent = period.Events.LastOrDefault();

                            var eventDateBasedOnTarget = (targetDate != null)
                                ? targetDate.Value.AddDays(_random.Next(-5, 5))
                                : (DateTime?)null;

                            var eventDate = (eventDateBasedOnTarget != null && lastEvent != null)
                                ? (eventDateBasedOnTarget >= lastEvent.EventDate
                                    ? (DateTime)eventDateBasedOnTarget
                                    : lastEvent.EventDate.AddDays(_random.Next(0, 5)))
                                : lastEvent.EventDate.AddDays(_random.Next(0, 5));

                            var @event = new ClockTickingEvent
                            {
                                Cancer = false,
                                Period = period,
                                Code = (EventCode)eventCode,
                                Comments = string.Empty,
                                EventDate = eventDate,
                                Clinician = clinicians[_random.Next(0, clinicians.Count - 1)],
                                IsActive = eventIndex == eventList.Count - 1,
                                TargetDate = targetDate
                            };
                            unitOfWork.Events.Add(@event);
                            Console.WriteLine("Added event {0} to UOW", @event.Code);
                        }
                    }

                    Console.WriteLine("Persisting UOW to DB...");
                    unitOfWork.SaveChanges();
                    Console.WriteLine("Done Adding events!");
                }
            }
        }

        private void GenerateEventConnections(IEnumerable<EventConnection> parentList, List<EventConnection> childList, ConfigurationEventCode parent)
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

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool AllocConsole();

        private void GenerateData_Load(object sender, EventArgs e)
        {
            AllocConsole();
        }
    }
}
