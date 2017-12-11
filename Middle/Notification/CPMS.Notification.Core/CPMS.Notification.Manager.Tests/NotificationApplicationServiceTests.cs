using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using CPMS.Patient.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace CPMS.Notification.Manager.Tests
{
    [TestClass]
    public class NotificationApplicationServiceTests
    {
        private Mock<BreachService> _breachServiceMock;
        private Mock<IClock> _clockMock;
        private Mock<IErrorRepository> _errorRepositoryMock;
        private NotificationApplicationService _notificationApplicationService;

        [TestInitialize]
        public void PerTestSetup()
        {
            _breachServiceMock = new Mock<BreachService>(null, null, null, null);
            _clockMock = new Mock<IClock>();
            _errorRepositoryMock = new Mock<IErrorRepository>();
            _notificationApplicationService = new NotificationApplicationService(_breachServiceMock.Object, _clockMock.Object, _errorRepositoryMock.Object);
        }

        [TestMethod]
        public void GetBreaches_CorectlyReturnsResult()
        {
            // Arrange
            var plannedEvent = CreatePlannedEvent();
            var @event = CreateEvent();
            _clockMock.Setup(c => c.TodayDate).Returns(new DateTime(2014, 3, 30));
            _breachServiceMock.Setup(c => c.GetPlannedBreachingEvents(It.IsAny<int>(), It.IsAny<BreachInput>()))
                .Returns(new[] { plannedEvent });
            _breachServiceMock.Setup(c => c.GetEventsForBreachingPeriods(It.IsAny<int>(), It.IsAny<BreachInput>()))
                .Returns(new[] { @event });

            // Act
            var result = _notificationApplicationService.GetBreaches();

            // Assert
            Assert.AreEqual(5, result.EventsBreaches.Count());
            Assert.AreEqual(4, result.PeriodsBreaches.Count());
        }

        [TestMethod]
        public void GetErrors_ReturnsListOfErrorInfo()
        {
            // Arrange
            var errors = new List<Error>
            {
                new Error {CreatedAt = new DateTime(2014, 2, 2), Message = "Msg1", Period = new RTT18WeekPeriod{Pathway = new Pathway{Patient = new Patient.Domain.Patient()}}}
            };
            _errorRepositoryMock.Setup(c => c.Get(It.IsAny<Expression<Func<Error, bool>>>())).Returns(errors);
            _clockMock.Setup(c => c.TodayDateAndTime).Returns(new DateTime(2014, 3, 30));

            // Act
            var result = _notificationApplicationService.GetErrors().ToList();

            // Assert
            Assert.AreEqual("Msg1", result[0].Message);
        }

        private PlannedEvent CreatePlannedEvent()
        {
            var parentEvent = new ClockStartingEvent
            {
                Code = EventCode.ReferralReview,
                Clinician = new Clinician { Name = "TestClinician", Specialty = new Specialty { Name = "TestSpecialty1" } }
            };

            var period = new RTT18WeekPeriod
            {
                Id = 1,
                Pathway =
                    new Pathway
                    {
                        PPINumber = "TestPPINumber",
                        Patient = new Patient.Domain.Patient { Name = "TestForename TestSurname", NHSNumber = "TestNHSNumber" }
                    }
            };
            period.Add(parentEvent);

            parentEvent.Period = period;

            return new PlannedEvent
            {
                Code = EventCode.ReferralReceived,
                TargetNumberOfDays = 2,
                IsMandatory = true,
                Event = parentEvent,
                DateReferenceForTarget = new DateTime(2014, 7, 3)
            };
        }

        private Event CreateEvent()
        {
            var ev = new ClockStartingEvent
            {
                Code = EventCode.ReferralReceived,
                Clinician =
                    new Clinician
                    {
                        Name = "TestClinician",
                        Specialty = new Specialty {Name = "TestSpecialty2"},
                        Hospital = new Hospital {Name = "TestHospital"}
                    },
                IsActive = true,
                TargetDate = new DateTime(2014, 4, 1),
                EventDate = new DateTime(2014, 4, 1),
                Period = new RTT18WeekPeriod
                {
                    Id = 1,
                    Pathway =
                        new Pathway
                        {
                            PPINumber = "TestPPINumber",
                            Patient =
                                new Patient.Domain.Patient
                                {
                                    Name = "TestForename TestSurname",
                                    NHSNumber = "TestNHSNumber",
                                    DateOfBirth = new DateTime(1991, 4, 15)
                                }
                        }
                }
            };
            ev.Period.Add(ev);

            return ev;
        }
    }
}
