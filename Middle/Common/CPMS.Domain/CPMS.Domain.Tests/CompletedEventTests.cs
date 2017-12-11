using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CPMS.Domain.Tests
{
    [TestClass]
    public class CompletedEventTests
    {
        [TestMethod]
        public void PostBreachDays_ReturnsNull_WhenTargetDateIsNull()
        {
            //Arrange
            var completedEvent = new ClockPausingCompletedEvent
            {
                Name = new EventName { Code = EventCode.InpatientTCIOffer },
                EventDate = new DateTime(2014, 8, 10)
            };

            //Act
            var result = completedEvent.PostBreachDays;

            //Assert
            Assert.AreEqual(null, result);
        }

        [TestMethod]
        public void PostBreachDays_ReturnsNull_WhenEventDateHappensBeforeTargetDate()
        {
            //Arrange
            var completedEvent = new ClockPausingCompletedEvent
            {
                Name = new EventName { Code = EventCode.InpatientTCIOffer },
                EventDate = new DateTime(2014, 8, 8),
                TargetDate = new DateTime(2014, 8, 9)
            };

            //Act
            var result = completedEvent.PostBreachDays;

            //Assert
            Assert.AreEqual(null, result);
        }

        [TestMethod]
        public void PostBreachDays_ReturnsCorrectNumberOfDays_WhenEventDateHappensAfterTargetDate()
        {
            //Arrange
            var completedEvent = new ClockPausingCompletedEvent
            {
                Name = new EventName { Code = EventCode.InpatientTCIOffer },
                EventDate = new DateTime(2014, 8, 12),
                TargetDate = new DateTime(2014, 8, 9)
            };

            //Act
            var result = completedEvent.PostBreachDays;

            //Assert
            Assert.AreEqual(3, result);
        }

        [TestMethod]
        public void EventStatus_ReturnCorrectEventStatus_WhenEventTypeIsClockPausingEvent()
        {
            //Arrange
            var completedEvent = new ClockPausingCompletedEvent
            {
                Name = new EventName { Code = EventCode.InpatientTCIOffer },
                EventDate = new DateTime(2014, 8, 10),
                TargetDate = new DateTime(2014, 8, 9)
            };

            //Act
            var result = completedEvent.EventStatus;

            //Assert
            Assert.AreEqual(EventStatus.Paused, result);
        }

        [TestMethod]
        public void EventStatus_ReturnCorrectEventStatus_WhenEventTypeIsClockStoppingEvent()
        {
            //Arrange
            var completedEvent = new ClockStoppingCompletedEvent
            {
                Name = new EventName { Code = EventCode.PathwayCompletion },
                EventDate = new DateTime(2014, 8, 10)
            };

            //Act
            var result = completedEvent.EventStatus;

            //Assert
            Assert.AreEqual(EventStatus.Closed, result);
        }

        [TestMethod]
        public void EventStatus_ReturnCorrectEventStatus_WhenEventTypeIsClockStartingEventAndIsNotBreached()
        {
            //Arrange
            var completedEvent = new ClockStartingCompletedEvent
            {
                Name = new EventName { Code = EventCode.ReferralReceived },
                EventDate = new DateTime(2014, 8, 10)
            };

            //Act
            var result = completedEvent.EventStatus;

            //Assert
            Assert.AreEqual(EventStatus.Success, result);
        }

        [TestMethod]
        public void EventStatus_ReturnCorrectEventStatus_WhenEventTypeIsClockStartingEventAndIsBreached()
        {
            //Arrange
            var completedEvent = new ClockStartingCompletedEvent
            {
                Name = new EventName { Code = EventCode.InpatientTCIAgreed },
                EventDate = new DateTime(2014, 8, 10),
                TargetDate = new DateTime(2014, 8, 9)
            };

            //Act
            var result = completedEvent.EventStatus;

            //Assert
            Assert.AreEqual(EventStatus.Breached, result);
        }

        [TestMethod]
        public void EventStatus_ReturnCorrectEventStatus_WhenEventTypeIsClockTickingEventAndIsNotBreached()
        {
            //Arrange
            var completedEvent = new ClockTickingCompletedEvent
            {
                Name = new EventName { Code = EventCode.ReferralReceived },
                EventDate = new DateTime(2014, 8, 10),
                IsBreached = false
            };

            //Act
            var result = completedEvent.EventStatus;

            //Assert
            Assert.AreEqual(EventStatus.Success, result);
        }

        [TestMethod]
        public void EventStatus_ReturnCorrectEventStatus_WhenEventTypeIsClockTickingEventAndIsBreached()
        {
            //Arrange
            var completedEvent = new ClockTickingCompletedEvent
            {
                Name = new EventName { Code = EventCode.InpatientTCIAgreed },
                EventDate = new DateTime(2014, 8, 10),
                TargetDate = new DateTime(2014, 8, 9),
                IsBreached = true
            };

            //Act
            var result = completedEvent.EventStatus;

            //Assert
            Assert.AreEqual(EventStatus.Breached, result);
        }

        [TestMethod]
        public void BreachStatus_ReturnCorrectBreachStatus_WhenEventStatusIsBreached()
        {
            //Arrange
            var completedEvent = new ClockTickingCompletedEvent
            {
                Name = new EventName { Code = EventCode.InpatientTCIAgreed },
                EventDate = new DateTime(2014, 8, 10),
                TargetDate = new DateTime(2014, 8, 9),
                IsBreached = true
            };

            //Act
            var result = completedEvent.BreachStatus;

            //Assert
            Assert.AreEqual(EventBreachStatus.Breached, result);
        }

        [TestMethod]
        public void BreachStatus_ReturnCorrectBreachStatus_WhenEventStatusIsNotBreached()
        {
            //Arrange
            var completedEvent = new ClockTickingCompletedEvent
            {
                Name = new EventName { Code = EventCode.InpatientTCIAgreed },
                EventDate = new DateTime(2014, 8, 10),
                TargetDate = new DateTime(2014, 8, 11)
            };

            //Act
            var result = completedEvent.BreachStatus;

            //Assert
            Assert.AreEqual(EventBreachStatus.Success, result);
        }

        [TestMethod]
        public void Validate_RaisesValidationFailedEvent_WhenClockPausesAndEventOtherThanInpatientTciAgreed()
        {
            //Arrange
            var completedEvent = new ClockPausingCompletedEvent { Name = new EventName { Code = EventCode.OutpatientDischarged }, Period = new RTT18WeekPeriod { Name = "period 1", Pathway = new Pathway { PPINumber = "ppi" } } };
            RuleViolation eventRaised = null;

            completedEvent.ValidationFailed += delegate { eventRaised = new RuleViolation(); };

            //Act
            completedEvent.Validate();

            //Assert
            Assert.IsNotNull(eventRaised);
        }

        [TestMethod]
        public void Validate_DoesNotRaiseValidationFailedEvent_WhenClockPausesAndEventInpatientTciAgreed()
        {
            //Arrange
            var completedEvent = new ClockPausingCompletedEvent { Name = new EventName { Code = EventCode.InpatientTCIOffer }, Period = new RTT18WeekPeriod { Name = "period 1", Pathway = new Pathway { PPINumber = "ppi" } } };
            RuleViolation eventRaised = null;

            completedEvent.ValidationFailed += delegate { eventRaised = new RuleViolation(); };

            //Act
            completedEvent.Validate();

            //Assert
            Assert.IsNull(eventRaised);
        }

        [TestMethod]
        public void Validate_RaisesValidationFailedEvent_WhenClockStopsAndEventOtherThanOutpatientDischarged()
        {
            //Arrange
            var completedEvent = new ClockStoppingCompletedEvent { Name = new EventName { Code = EventCode.InpatientAdmitted }, Period = new RTT18WeekPeriod { Name = "period 1", Pathway = new Pathway { PPINumber = "ppi" } } };
            RuleViolation eventRaised = null;

            completedEvent.ValidationFailed += delegate { eventRaised = new RuleViolation(); };

            //Act
            completedEvent.Validate();

            //Assert
            Assert.IsNotNull(eventRaised);
        }

        [TestMethod]
        public void Validate_DoesNotRaiseValidationFailedEvent_WhenClockStopsAndEventOutpatientDischarged()
        {
            //Arrange
            var completedEvent = new ClockStoppingCompletedEvent { Name = new EventName { Code = EventCode.OutpatientDischarged }, Period = new RTT18WeekPeriod { Name = "period 1", Pathway = new Pathway { PPINumber = "ppi" } } };
            RuleViolation eventRaised = null;

            completedEvent.ValidationFailed += delegate { eventRaised = new RuleViolation(); };

            //Act
            completedEvent.Validate();

            //Assert
            Assert.IsNull(eventRaised);
        }

        [TestMethod]
        public void Validate_RaisesValidationFailedEvent_WhenClockStopsAndEventOtherThanInpatientDischarged()
        {
            //Arrange
            var completedEvent = new ClockStoppingCompletedEvent { Name = new EventName { Code = EventCode.PatientAddedToInpatientWaitingList }, Period = new RTT18WeekPeriod { Name = "period 1", Pathway = new Pathway { PPINumber = "ppi" } } };
            var eventRaised = false;

            completedEvent.ValidationFailed += delegate { eventRaised = true; };

            //Act
            completedEvent.Validate();

            //Assert
            Assert.IsTrue(eventRaised);
        }

        [TestMethod]
        public void Validate_DoesNotRaiseValidationFailedEvent_WhenClockStopsAndEventInpatientDischarged()
        {
            //Arrange
            var completedEvent = new ClockStoppingCompletedEvent { Name = new EventName { Code = EventCode.InpatientDischarged }, Period = new RTT18WeekPeriod { Name = "period 1", Pathway = new Pathway { PPINumber = "ppi" } } };
            var eventRaised = false;

            completedEvent.ValidationFailed += delegate { eventRaised = true; };

            //Act
            completedEvent.Validate();

            //Assert
            Assert.IsFalse(eventRaised);
        }

        [TestMethod]
        public void Validate_RaisesValidationFailedEvent_WhenClockStopsAndEventOtherThanPathwayCompletion()
        {
            //Arrange
            var completedEvent = new ClockStoppingCompletedEvent { Name = new EventName { Code = EventCode.DiagnosticsOrderPlaced }, Period = new RTT18WeekPeriod { Name = "period 1", Pathway = new Pathway { PPINumber = "ppi" } } };
            RuleViolation eventRaised = null;

            completedEvent.ValidationFailed += delegate { eventRaised = new RuleViolation(); };

            //Act
            completedEvent.Validate();

            //Assert
            Assert.IsNotNull(eventRaised);
        }

        [TestMethod]
        public void Validate_DoesNotRaiseValidationFailedEvent_WhenClockStopsAndEventPathwayCompletion()
        {
            //Arrange
            var completedEvent = new ClockStoppingCompletedEvent { Name = new EventName { Code = EventCode.PathwayCompletion }, Period = new RTT18WeekPeriod { Name = "period 1", Pathway = new Pathway { PPINumber = "ppi" } } };
            var eventRaised = false;

            completedEvent.ValidationFailed += delegate { eventRaised = true; };

            //Act
            completedEvent.Validate();

            //Assert
            Assert.IsFalse(eventRaised);
        }

        [TestMethod]
        public void Validate_RaisesValidationFailedEvent_WhenCancerPeriodAndEventPausesPeriod()
        {
            // Arrange
            var completedEvent = new ClockPausingCompletedEvent { Name = new EventName { Code = EventCode.InpatientTCIOffer }, EventDate = new DateTime(2014, 12, 26), Period = new CancerPeriod { Name = "period 1", Pathway = new Pathway { PPINumber = "ppi" } } };
            var eventRaised = false;

            completedEvent.ValidationFailed += delegate { eventRaised = true; };

            // Act
            completedEvent.Validate();

            // Assert
            Assert.IsTrue(eventRaised);
        }
    }
}
