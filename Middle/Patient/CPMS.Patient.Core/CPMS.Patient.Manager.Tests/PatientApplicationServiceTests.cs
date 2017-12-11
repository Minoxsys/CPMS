//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Linq.Expressions;
//using CPMS.Patient.Domain;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using Moq;

//namespace CPMS.Patient.Manager.Tests
//{
//    [TestClass]
//    public class PatientApplicationServiceTests
//    {
//        private Mock<BreachService> _breachServiceMock;
//        private PatientApplicationService _patientApplicationService;
//        private Mock<IClock> _clockMock;
//        private Mock<IMapper<BreachInputInfo, BreachInput>> _breachInputInfoToBreachInputMapperMock;
//        private Mock<IMapper<Hospital, HospitalInfo>> _hospitalToHospitalInfoMapperMock;
//        private Mock<IMapper<Specialty, SpecialtyInfo>> _specialtyToSpecialtyInfoMapperMock;
//        private Mock<IMapper<Clinician, ClinicianInfo>> _clinicianToClinicianInfoMapperMock;
//        private Mock<IMapper<Pathway, PathwayInfo>> _pathwayToPathwayInfoMapperMock;
//        private Mock<IMapper<PeriodEventsInputInfo, PeriodEventsInput>> _periodEventsInputInfoToPeriodEventsInputMapperMock;
//        private Mock<IMapper<Event, EventHistoryLogInfo>> _eventToEventHistoryLogInfoMapperMock;
//        private Mock<IMapper<ListInputInfo, ListInput>> _listInputInfoToListInputMapperMock;
//        private Mock<IHospitalRepository> _hospitalRepositoryMock;
//        private Mock<ISpecialtyRepository> _specialtyRepositoryMock;
//        private Mock<IClinicianRepository> _clinicianRepositoryMock;
//        private Mock<IEventRepository> _eventRepositoryMock;
//        private Mock<IPathwayRepository> _pathwayRepositoryMock;
//        private Mock<IPeriodRepository> _periodRepositoryMock;
//        private Mock<IPlannedEventRepository> _plannedEventRepositoryMock;
        
//        [TestInitialize]
//        public void PerTestSetup()
//        {
//            _breachServiceMock = new Mock<BreachService>(null, null, null, null);
//            _clockMock = new Mock<IClock>();
//            _breachInputInfoToBreachInputMapperMock = new Mock<IMapper<BreachInputInfo, BreachInput>>();
//            _hospitalToHospitalInfoMapperMock = new Mock<IMapper<Hospital, HospitalInfo>>();
//            _specialtyToSpecialtyInfoMapperMock = new Mock<IMapper<Specialty, SpecialtyInfo>>();
//            _clinicianToClinicianInfoMapperMock = new Mock<IMapper<Clinician, ClinicianInfo>>();
//            _pathwayToPathwayInfoMapperMock = new Mock<IMapper<Pathway, PathwayInfo>>();
//            _periodEventsInputInfoToPeriodEventsInputMapperMock = new Mock<IMapper<PeriodEventsInputInfo, PeriodEventsInput>>();
//            _eventToEventHistoryLogInfoMapperMock = new Mock<IMapper<Event, EventHistoryLogInfo>>();
//            _listInputInfoToListInputMapperMock = new Mock<IMapper<ListInputInfo, ListInput>>();
//            _hospitalRepositoryMock = new Mock<IHospitalRepository>();
//            _specialtyRepositoryMock = new Mock<ISpecialtyRepository>();
//            _clinicianRepositoryMock = new Mock<IClinicianRepository>();
//            _eventRepositoryMock = new Mock<IEventRepository>();
//            _pathwayRepositoryMock = new Mock<IPathwayRepository>();
//            _periodRepositoryMock = new Mock<IPeriodRepository>();
//            _plannedEventRepositoryMock = new Mock<IPlannedEventRepository>();

//            _patientApplicationService = new PatientApplicationService(_breachServiceMock.Object,
//                _clockMock.Object,
//                _breachInputInfoToBreachInputMapperMock.Object,
//                _hospitalToHospitalInfoMapperMock.Object,
//                _specialtyToSpecialtyInfoMapperMock.Object,
//                _clinicianToClinicianInfoMapperMock.Object,
//                _pathwayToPathwayInfoMapperMock.Object,
//                _eventToEventHistoryLogInfoMapperMock.Object,
//                _periodEventsInputInfoToPeriodEventsInputMapperMock.Object,
//                _listInputInfoToListInputMapperMock.Object,
//                _hospitalRepositoryMock.Object,
//                _specialtyRepositoryMock.Object,
//                _clinicianRepositoryMock.Object,
//                _eventRepositoryMock.Object,
//                _pathwayRepositoryMock.Object,
//                _periodRepositoryMock.Object, 
//                _plannedEventRepositoryMock.Object);
//        }

//        [TestMethod]
//        public void GetBreachingEvents_CorrectlyReturnsEventsBreachInfo()
//        {
//            // Arrange
//            var events = new[]
//            {
//                CreatePlannedEvent()
//            };
//            var breachInput = new BreachInput
//            {
//                BreachFilterInput = new BreachFilterInput(),
//                ListInput = new ListInput()
//            };
//            _breachInputInfoToBreachInputMapperMock.Setup(m => m.Map(It.IsAny<BreachInputInfo>()))
//                .Returns(breachInput);
//            _clockMock.Setup(c => c.TodayDate).Returns(new DateTime(2014, 5, 12));
//            _breachServiceMock.Setup(m => m.GetPlannedBreachingEvents(3, breachInput)).Returns(events);

//            // Act
//            var result = _patientApplicationService.GetEventBreaches(3, null);
//            var firstEventInfo = result.EventsInfo.First();

//            // Assert
//            Assert.IsTrue(firstEventInfo.EventCode == EventCode.ReferralReceived &&
//                          firstEventInfo.PatientNHSNumber == "TestNHSNumber" &&
//                          firstEventInfo.PatientName == "TestForename TestSurname" &&
//                          firstEventInfo.PPINumber == "TestPPINumber" &&
//                          firstEventInfo.Clinician == "TestClinician" &&
//                          firstEventInfo.Specialty == "TestSpecialty1");
//        }

//        [TestMethod]
//        public void GetBreachingPeriods_CorrectlyReturnsEventsForPeriodBreaches()
//        {
//            // Arrange
//            var events = new[]
//            {
//                CreateEvent()
//            };
//            var breachInput = new BreachInput
//            {
//                BreachFilterInput = new BreachFilterInput(),
//                ListInput = new ListInput()
//            };
//            _breachInputInfoToBreachInputMapperMock.Setup(m => m.Map(It.IsAny<BreachInputInfo>()))
//                .Returns(breachInput);
//            _clockMock.Setup(c => c.TodayDate).Returns(new DateTime(2014, 8, 2));
//            _breachServiceMock.Setup(m => m.GetEventsForBreachingPeriods(3, breachInput)).Returns(events);
//            _breachServiceMock.Setup(m => m.CountEventsForBreachingPeriods(3, null)).Returns(1);

//            // Act
//            var result = _patientApplicationService.GetPeriodBreaches(3, null);
//            var firstPeriodInfo = result.PeriodsInfo.First();

//            // Assert
//            Assert.IsTrue(firstPeriodInfo.EventCode == EventCode.ReferralReceived &&
//                          firstPeriodInfo.PatientNHSNumber == "TestNHSNumber" &&
//                          firstPeriodInfo.PatientName == "TestForename TestSurname" &&
//                          firstPeriodInfo.PPINumber == "TestPPINumber" &&
//                          firstPeriodInfo.Clinician == "TestClinician" &&
//                          firstPeriodInfo.Specialty == "TestSpecialty2");
//        }

//        [TestMethod]
//        public void GetHospitals_ReturnsListOfHospitalInfos()
//        {
//            // Arrange
//            const int id = 1;
//            const string name = "Hospital test";
//            var hospital = new Hospital { Id = id, Name = name };
//            var hospitalInfo = new HospitalInfo { Id = id, Name = name };
//            _hospitalRepositoryMock.Setup(c => c.Get(It.IsAny<Expression<Func<Hospital, bool>>>())).Returns(new List<Hospital> { hospital });
//            _hospitalToHospitalInfoMapperMock.Setup(c => c.Map(hospital)).Returns(hospitalInfo);

//            // Act
//            var result = _patientApplicationService.GetHospitals();

//            // Assert
//            Assert.AreEqual(hospitalInfo.Name, result.FirstOrDefault().Name);
//        }

//        [TestMethod]
//        public void GetSpecialties_ReturnsListOfSpecialtyInfos()
//        {
//            // Arrange
//            const string code = "1";
//            const string name = "Specialty test";
//            var specialty = new Specialty { Code = code, Name = name };
//            var specialtyInfo = new SpecialtyInfo { Code = code, Name = name };
//            _specialtyRepositoryMock.Setup(c => c.Get(It.IsAny<Expression<Func<Specialty, bool>>>())).Returns(new List<Specialty> { specialty });
//            _specialtyToSpecialtyInfoMapperMock.Setup(c => c.Map(specialty)).Returns(specialtyInfo);

//            // Act
//            var result = _patientApplicationService.GetSpecialties(It.IsAny<int?>());

//            // Assert
//            Assert.AreEqual(specialtyInfo.Name, result.FirstOrDefault().Name);
//        }

//        [TestMethod]
//        public void GetClinicians_ReturnsListOfClinicianInfos()
//        {
//            // Arrange
//            const int id = 1;
//            const string name = "Clinician test";
//            var clinician = new Clinician { Id = id, Name = name };
//            var clinicianInfo = new ClinicianInfo { Id = id, Name = name };
//            _clinicianRepositoryMock.Setup(c => c.Get(It.IsAny<Expression<Func<Clinician, bool>>>())).Returns(new List<Clinician> { clinician });
//            _clinicianToClinicianInfoMapperMock.Setup(c => c.Map(clinician)).Returns(clinicianInfo);

//            // Act
//            var result = _patientApplicationService.GetClinicians(It.IsAny<int?>(), It.IsAny<string>());

//            // Assert
//            Assert.AreEqual(clinicianInfo.Name, result.FirstOrDefault().Name);
//        }

//        [TestMethod]
//        public void GetPatientsOnPathway_CorectlyReturnsPatientsInfo()
//        {
//            //Arrange
//            var events = new[]
//            {
//                CreateEvent()
//            };
//            var listInput = new ListInput();
//            _listInputInfoToListInputMapperMock.Setup(e => e.Map(It.IsAny<ListInputInfo>())).Returns(listInput);
//            _clockMock.Setup(c => c.TodayDate).Returns(new DateTime(2014, 8, 2));
//            _eventRepositoryMock.Setup(m => m.Get(It.IsAny<Expression<Func<Event, bool>>>(), listInput)).Returns(events);
//            _eventRepositoryMock.Setup(m => m.Count(It.IsAny<Expression<Func<Event, bool>>>())).Returns(1);

//            //Act 
//            var result = _patientApplicationService.GetPatientsOnPathway(new PatientInputInfo());
//            var firstPatientInfo = result.PatientInfo.First();

//            //Assert
//            Assert.IsTrue(firstPatientInfo.Age == 23 &&
//                firstPatientInfo.DateOfBirth == new DateTime(1991, 4, 15) &&
//                firstPatientInfo.PatientName == "TestForename TestSurname" &&
//                firstPatientInfo.Hospital == "TestHospital" &&
//                firstPatientInfo.PpiNumber == "TestPPINumber" &&
//                firstPatientInfo.NHSNumber == "TestNHSNumber");
//        }

//        [TestMethod]
//        public void GetPathwaysForPatient_ReturnsListOfPathwayInfos()
//        {
//            // Arrange
//            const string ppiNumber = "TestPPI";
//            var pathway = new Pathway { PPINumber = ppiNumber };
//            var pathwayInfo = new PathwayInfo{ PPINumber = ppiNumber };

//            _pathwayRepositoryMock.Setup(c => c.Get(It.IsAny<Expression<Func<Pathway, bool>>>())).Returns(new List<Pathway> { pathway });
//            _pathwayToPathwayInfoMapperMock.Setup(c => c.Map(pathway)).Returns(pathwayInfo);

//            // Act
//            var result = _patientApplicationService.GetPathwaysForPatient(It.IsAny<string>());

//            // Assert
//            Assert.AreEqual(pathwayInfo.PPINumber, result.FirstOrDefault().PPINumber);
//        }

//        [TestMethod]
//        public void GetPeriodsForPathway_ReturnsListOfPeriodInfos()
//        {
//            // Arrange
//            const string periodName = "TestName";
//            const int id = 1;
//            var period = new RTT18WeekPeriod { Name = periodName, Id = id, StartDate = new DateTime(2014, 4, 15), StopDate = new DateTime(2014, 8, 15), Pathway = new Pathway(), Events = new List<Event>{new ClockStartingEvent()}};
//            var periodInfo = new PeriodInfo { Name = periodName, Id = id, StartDate = new DateTime(2014, 4, 15), StopDate = new DateTime(2014, 8, 15) };

//            _periodRepositoryMock.Setup(c => c.Get(It.IsAny<Expression<Func<Period, bool>>>())).Returns(new List<Period> { period });
//            _breachServiceMock.Setup(c => c.IsPeriodBreached(It.IsAny<int>())).Returns(false);
//            // Act
//            var firstPeriod = _patientApplicationService.GetPeriodsForPathway(It.IsAny<string>()).FirstOrDefault();

//            // Assert
//            Assert.AreEqual(periodInfo.Name, firstPeriod.Name);
//            Assert.AreEqual(periodInfo.Id, firstPeriod.Id);
//            Assert.AreEqual(periodInfo.StartDate, firstPeriod.StartDate);
//            Assert.AreEqual(periodInfo.StopDate, firstPeriod.StopDate);
//        }

//        [TestMethod]
//        public void GetPeriodEvents_ReturnsListOfPeriodEventsInfo_WhenPeriodEventsInputInfoIsEmpty()
//        {
//            //Arrange
//            var events = new[]
//            {
//                CreateEvent()
//            };
//            var plannedEvents = new[]
//            {
//                CreatePlannedEvent()
//            };
//            const int id = 3;
//            _periodEventsInputInfoToPeriodEventsInputMapperMock.Setup(m => m.Map(It.IsAny<PeriodEventsInputInfo>()))
//               .Returns(new PeriodEventsInput());
//            _breachServiceMock.Setup(m => m.GetPeriodEvents(id, It.IsAny<PeriodEventsInput>())).Returns(events);
//            _breachServiceMock.Setup(m => m.GetPeriodPlannedEvents(id, It.IsAny<PeriodEventsInput>())).Returns(plannedEvents);

//            //Act
//            var periodEventsInfo = _patientApplicationService.GetPeriodEvents(id, new PeriodEventsInputInfo());

//            //Assert
//            Assert.AreEqual(periodEventsInfo.TotalNumberOfEvents, 2);
//        }

//        [TestMethod]
//        public void GetPeriodEvents_ReturnsListOfPeriodEventsInfo_WhenListInputInfoHasOrderBySpecialtyAscending()
//        {
//            //Arrange
//            var events = new[]
//            {
//                CreateEvent()
//            };
//            var plannedEvents = new[]
//            {
//                CreatePlannedEvent()
//            };
//            var periodEventsInput = new PeriodEventsInput
//            {
//                ListInput = new ListInput
//                {
//                    Index = 0,
//                    PageCount = 5,
//                    OrderBy = OrderBy.Specialty,
//                    OrderDirection = OrderDirection.Ascending
//                },
//                PeriodEventsFilterInput = new PeriodEventsFilterInput()
//            };
//            const int id = 3;
//            _periodEventsInputInfoToPeriodEventsInputMapperMock.Setup(m => m.Map(It.IsAny<PeriodEventsInputInfo>()))
//               .Returns(periodEventsInput);
//            _breachServiceMock.Setup(m => m.GetPeriodEvents(id, It.IsAny<PeriodEventsInput>())).Returns(events);
//            _breachServiceMock.Setup(m => m.GetPeriodPlannedEvents(id, It.IsAny<PeriodEventsInput>())).Returns(plannedEvents);

//            //Act
//            var periodEventsInfo = _patientApplicationService.GetPeriodEvents(id, new PeriodEventsInputInfo());

//            //Assert
//            Assert.AreEqual(periodEventsInfo.TotalNumberOfEvents, 2);
//            Assert.AreEqual(periodEventsInfo.Events.First().Specialty, "TestSpecialty1");
//            Assert.AreEqual(periodEventsInfo.Events.Last().Specialty, "TestSpecialty2");
//        }

//        [TestMethod]
//        public void GetPeriodEvents_ReturnsListOfPeriodEventsInfo_WhenListInputInfoHasOrderBySpecialtyDescending()
//        {
//            //Arrange
//            var events = new[]
//            {
//                CreateEvent()
//            };
//            var plannedEvents = new[]
//            {
//                CreatePlannedEvent()
//            };
//            var periodEventsInput = new PeriodEventsInput
//            {
//                ListInput = new ListInput
//                {
//                    Index = 0,
//                    PageCount = 5,
//                    OrderBy = OrderBy.Specialty,
//                    OrderDirection = OrderDirection.Descending
//                },
//                PeriodEventsFilterInput = new PeriodEventsFilterInput()
//            };
//            const int id = 3;
//            _periodEventsInputInfoToPeriodEventsInputMapperMock.Setup(m => m.Map(It.IsAny<PeriodEventsInputInfo>()))
//               .Returns(periodEventsInput);
//            _breachServiceMock.Setup(m => m.GetPeriodEvents(id, It.IsAny<PeriodEventsInput>())).Returns(events);
//            _breachServiceMock.Setup(m => m.GetPeriodPlannedEvents(id, It.IsAny<PeriodEventsInput>())).Returns(plannedEvents);
//            _breachServiceMock.Setup(m => m.IsPeriodBreached(It.IsAny<int>())).Returns(true);

//            //Act
//            var periodEventsInfo = _patientApplicationService.GetPeriodEvents(id, new PeriodEventsInputInfo());

//            //Assert
//            Assert.AreEqual(periodEventsInfo.TotalNumberOfEvents, 2);
//            Assert.AreEqual(periodEventsInfo.Events.First().Specialty, "TestSpecialty2");
//            Assert.AreEqual(periodEventsInfo.Events.Last().Specialty, "TestSpecialty1"); 
//            Assert.AreEqual(periodEventsInfo.IsBreached, true);
//        }
        
//        [TestMethod]
//        public void GetLiteEventBreaches_CorrectlyReturnsResult_WhenPlannedEventIsIn3DaysToBreachRange()
//        {
//            // Arrange
//            var plannedEvent = CreatePlannedEvent();
//            _clockMock.Setup(m => m.TodayDate).Returns(new DateTime(2014, 7, 4));
//            _breachServiceMock.Setup(m => m.GetPeriodPlannedEvents(7, It.IsAny<PeriodEventsInput>()))
//                .Returns(new[] {plannedEvent});

//            // Act
//            var result = _patientApplicationService.GetLiteEventBreaches(7).ToArray();

//            // Assert
//            Assert.IsTrue(result.Count() == 1 && result.First().EventCode == EventCode.ReferralReceived &&
//                          result.First().Status == EventBreachStatus.AboutToBreach && result.First().DaysForStatus == 1);
//        }

//        [TestMethod]
//        public void GetLiteEventBreaches_CorrectlyReturnsResult_WhenPlannedEventIsNotIn3DaysToBreachRange()
//        {
//            // Arrange
//            var plannedEvent = CreatePlannedEvent();
//            _clockMock.Setup(m => m.TodayDate).Returns(new DateTime(2014, 7, 1));
//            _breachServiceMock.Setup(m => m.GetPeriodPlannedEvents(7, It.IsAny<PeriodEventsInput>()))
//                .Returns(new[] { plannedEvent });

//            // Act
//            var result = _patientApplicationService.GetLiteEventBreaches(7).ToArray();

//            // Assert
//            Assert.IsTrue(!result.Any());
//        }

//        [TestMethod]
//        public void GetLiteEventBreaches_ReturnsAbsoluteValueForNumberOfDays_WhenPlannedEventIsBreached()
//        {
//            // Arrange
//            var plannedEvent = CreatePlannedEvent();
//            _clockMock.Setup(m => m.TodayDate).Returns(new DateTime(2014, 7, 8));
//            _breachServiceMock.Setup(m => m.GetPeriodPlannedEvents(7, It.IsAny<PeriodEventsInput>()))
//                .Returns(new[] { plannedEvent });

//            // Act
//            var result = _patientApplicationService.GetLiteEventBreaches(7).ToArray();

//            // Assert
//            Assert.IsTrue(result.Count() == 1 && result.First().EventCode == EventCode.ReferralReceived &&
//                          result.First().Status == EventBreachStatus.Breached && result.First().DaysForStatus == 3);
//        }

//        [TestMethod]
//        public void GetEventHistoryLog_CorectlyReturnsResult()
//        {
//            // Arrange
//            var eventInfo = CreateEventsHistoryLogInfo();
//            var events = new[]
//            {
//                CreateEvent()
//            };
//            _listInputInfoToListInputMapperMock.Setup(e => e.Map(It.IsAny<ListInputInfo>())).Returns(new ListInput());
//            _eventRepositoryMock.Setup(e => e.Get(It.IsAny<Expression<Func<Event, bool>>>(), It.IsAny<ListInput>())).Returns(events);
//            _eventRepositoryMock.Setup(m => m.Count(It.IsAny<Expression<Func<Event, bool>>>())).Returns(1);
//            foreach (var @event in events)
//            {
//                _eventToEventHistoryLogInfoMapperMock.Setup(e => e.Map(@event)).Returns(eventInfo);
//            }

//            // Act
//            var result = _patientApplicationService.GetEventHistoryLog(1, new EventHistoryLogInputInfo());

//            // Assert
//            Assert.AreEqual(result.EventsInfo.Count(), 1);
//            Assert.AreEqual(result.TotalNumberOfEvents, 1);
//        }

//        [TestMethod]
//        public void GetEventsCounter_DoesNotCountEventsThatStartsOrClosesThePeriod()
//        {
//            // Arrange
//            var plannedEvents = new[] {CreatePlannedEvent()};
//            var events = new[] {CreateEvent()};
//            _plannedEventRepositoryMock.Setup(p => p.Get(It.IsAny<Expression<Func<PlannedEvent, bool>>>(), It.IsAny<ListInput>())).Returns(plannedEvents);
//            _eventRepositoryMock.Setup(p => p.Get(It.IsAny<Expression<Func<Event, bool>>>(), It.IsAny<ListInput>())).Returns(events);

//            // Act
//            var eventsCounter = _patientApplicationService.GetEventsCounter().ToArray();

//            // Assert
//            Assert.IsNull(eventsCounter.FirstOrDefault(e => e.EventCode == EventCode.ReferralReceived));
//            Assert.IsNull(eventsCounter.FirstOrDefault(e => e.EventCode == EventCode.PathwayCompletion));
//            Assert.IsNull(eventsCounter.FirstOrDefault(e => e.EventCode == EventCode.PatientCancelEvent));
//            Assert.IsNull(eventsCounter.FirstOrDefault(e => e.EventCode == EventCode.HospitalCancelEvent));
//            Assert.IsNull(eventsCounter.FirstOrDefault(e => e.EventCode == EventCode.ReferralReview));
//            Assert.IsNull(eventsCounter.FirstOrDefault(e => e.EventCode == EventCode.DiagnosticsOrderPlaced));
//            Assert.IsNull(eventsCounter.FirstOrDefault(e => e.EventCode == EventCode.BookedOutpatientFollowUpAppointment));
//            Assert.IsNull(eventsCounter.FirstOrDefault(e => e.EventCode == EventCode.BookedPostInpatientFirstOutpatientAppointment));
//            Assert.IsNull(eventsCounter.FirstOrDefault(e => e.EventCode == EventCode.BookedPostInpatientSecondOutpatientAppointment));
//        }

//        [TestMethod]
//        public void GetPeriodCounterForTrust_CorectlyReturnsPeriodCounter()
//        {
//            // Arrange
//            var periodCounter = new PeriodCounter
//            {
//                AboutToBreachNumber = 1,
//                BreachedNumber = 1
//            };
//            _breachServiceMock.Setup(p => p.GetPeriodCounter(It.IsAny<int?>(), It.IsAny<string>(), It.IsAny<int?>())).Returns(periodCounter);
//            _hospitalRepositoryMock.Setup(h => h.Get(It.IsAny<Expression<Func<Hospital, bool>>>()))
//                .Returns(new[] {new Hospital {Id = 1, Name = "hospital"}});

//            // Act
//            var firstPeriodCounter = _patientApplicationService.GetPeriodCounterForPathwayTypes().First();

//            // Assert
//            Assert.AreEqual(1, firstPeriodCounter.AboutToBreachNumber);
//            Assert.AreEqual(1, firstPeriodCounter.BreachedNumber);
//            Assert.AreEqual("1", firstPeriodCounter.Id);
//            Assert.AreEqual("hospital", firstPeriodCounter.Name);
//        }

//        [TestMethod]
//        public void GetPeriodCounterForHospital_CorectlyReturnsPeriodCounterForHospitalId()
//        {
//            // Arrange
//            var periodCounter = new PeriodCounter
//            {
//                AboutToBreachNumber = 1,
//                BreachedNumber = 1
//            };
//            _breachServiceMock.Setup(p => p.GetPeriodCounter(It.IsAny<int?>(), It.IsAny<string>(), It.IsAny<int?>())).Returns(periodCounter);
//            _specialtyRepositoryMock.Setup(h => h.Get(It.IsAny<Expression<Func<Specialty, bool>>>()))
//                .Returns(new[] { new Specialty{ Code = "code", Name = "specialty" } });

//            // Act
//            var firstPeriodCounter = _patientApplicationService.GetPeriodCounterForHospital("1").First();

//            // Assert
//            Assert.AreEqual(1, firstPeriodCounter.AboutToBreachNumber);
//            Assert.AreEqual(1, firstPeriodCounter.BreachedNumber);
//            Assert.AreEqual("code", firstPeriodCounter.Id);
//            Assert.AreEqual("specialty", firstPeriodCounter.Name);
//        }

//        [TestMethod]
//        public void GetPeriodCounterForSpecialty_CorectlyReturnsPeriodCounterForSpecialtyId()
//        {
//            // Arrange
//            var periodCounter = new PeriodCounter
//            {
//                AboutToBreachNumber = 1,
//                BreachedNumber = 1
//            };
//            _breachServiceMock.Setup(p => p.GetPeriodCounter(It.IsAny<int?>(), It.IsAny<string>(), It.IsAny<int?>())).Returns(periodCounter);
//            _clinicianRepositoryMock.Setup(h => h.Get(It.IsAny<Expression<Func<Clinician, bool>>>()))
//                .Returns(new[] { new Clinician{ Id = 1, Name = "clinician" } });

//            // Act
//            var firstPeriodCounter = _patientApplicationService.GetPeriodCounterForSpecialty("1", "hospitalId").First();

//            // Assert
//            Assert.AreEqual(1, firstPeriodCounter.AboutToBreachNumber);
//            Assert.AreEqual(1, firstPeriodCounter.BreachedNumber);
//            Assert.AreEqual("1", firstPeriodCounter.Id);
//            Assert.AreEqual("clinician", firstPeriodCounter.Name);
//        }

//        private EventHistoryLogInfo CreateEventsHistoryLogInfo()
//        {
//            return new EventHistoryLogInfo
//            {
//                EventCode = EventCode.ReferralReceived,
//                Description = "description",
//                ActualDate = new DateTime(2014, 7, 3),
//                TargetDate = new DateTime(2014, 7, 3),
//                ImportDate = new DateTime(2014, 7, 3)
//            };

//        }

//        private PlannedEvent CreatePlannedEvent()
//        {
//            var parentEvent = new ClockStartingEvent
//            {
//                Code = EventCode.ReferralReview,
//                Clinician = new Clinician { Name = "TestClinician", Specialty = new Specialty { Name = "TestSpecialty1" } },
//                EventDate = new DateTime(2014, 7, 3)
//            };

//            var period = new RTT18WeekPeriod
//            {
//                Id = 1,
//                Pathway =
//                    new Pathway
//                    {
//                        PPINumber = "TestPPINumber",
//                        Patient = new Domain.Patient { Name = "TestForename TestSurname", NHSNumber = "TestNHSNumber" }
//                    }
//            };
//            period.Add(parentEvent);

//            parentEvent.Period = period;

//            return new PlannedEvent
//            {
//                Code = EventCode.ReferralReceived,
//                TargetNumberOfDays = 2,
//                IsMandatory = true,
//                Event = parentEvent,
//                DateReferenceForTarget = new DateTime(2014, 7, 3)
//            };
//        }

//        private Event CreateEvent()
//        {
//            var ev = new ClockStartingEvent
//            {
//                Code = EventCode.ReferralReceived,
//                Clinician = new Clinician { Name = "TestClinician", Specialty = new Specialty { Name = "TestSpecialty2" }, Hospital = new Hospital { Name = "TestHospital" } },
//                IsActive = true,
//                TargetDate = new DateTime(2014, 4, 1),
//                EventDate = new DateTime(2014, 4, 1)
//            };
//            ev.Period = new RTT18WeekPeriod
//            {
//                Id = 1,
//                Pathway =
//                    new Pathway
//                    {
//                        PPINumber = "TestPPINumber",
//                        Patient =
//                            new Domain.Patient { Name = "TestForename TestSurname", NHSNumber = "TestNHSNumber", DateOfBirth = new DateTime(1991, 4, 15) }
//                    }
//            };
//            ev.Period.Add(ev);

//            return ev;
//        }
        
//    }
//}
