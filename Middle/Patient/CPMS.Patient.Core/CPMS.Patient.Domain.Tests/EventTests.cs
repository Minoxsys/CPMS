using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CPMS.Patient.Domain.Tests
{
    [TestClass]
    public class EventTests
    {
        [TestMethod]
        public void PostBreachDays_ReturnsNull_WhenTargetDateIsNull()
        {
            //Arrange
            var @event = new ClockPausingEvent
            {
                Code = EventCode.InpatientTCIOffer,
                EventDate = new DateTime(2014, 8, 10)
            };

            //Act
            var result = @event.PostBreachDays;

            //Assert
            Assert.AreEqual(null, result);
        }

        [TestMethod]
        public void PostBreachDays_ReturnsNull_WhenEventDateHappensBeforeTargetDate()
        {
            //Arrange
            var @event = new ClockPausingEvent
            {
                Code = EventCode.InpatientTCIOffer,
                EventDate = new DateTime(2014, 8, 8),
                TargetDate = new DateTime(2014, 8, 9)
            };

            //Act
            var result = @event.PostBreachDays;

            //Assert
            Assert.AreEqual(null, result);
        }

        [TestMethod]
        public void PostBreachDays_ReturnsCorrectNumberOfDays_WhenEventDateHappensAfterTargetDate()
        {
            //Arrange
            var @event = new ClockPausingEvent
            {
                Code = EventCode.InpatientTCIOffer,
                EventDate = new DateTime(2014, 8, 12),
                TargetDate = new DateTime(2014, 8, 9)
            };

            //Act
            var result = @event.PostBreachDays;

            //Assert
            Assert.AreEqual(3, result);
        }

        [TestMethod]
        public void EventStatus_ReturnCorrectEventStatus_WhenEventTypeIsClockPausingEvent()
        {
            //Arrange
            var @event = new ClockPausingEvent
            {
                Code = EventCode.InpatientTCIOffer,
                EventDate = new DateTime(2014, 8, 10),
                TargetDate = new DateTime(2014, 8, 9)
            };

            //Act
            var result = @event.EventStatus;

            //Assert
            Assert.AreEqual(EventStatus.Paused, result);
        }

        [TestMethod]
        public void EventStatus_ReturnCorrectEventStatus_WhenEventTypeIsClockStoppingEvent()
        {
            //Arrange
            var @event = new ClockStoppingEvent
            {
                Code = EventCode.PathwayCompletion,
                EventDate = new DateTime(2014, 8, 10)
            };

            //Act
            var result = @event.EventStatus;

            //Assert
            Assert.AreEqual(EventStatus.Closed, result);
        }

        [TestMethod]
        public void EventStatus_ReturnCorrectEventStatus_WhenEventTypeIsClockStartingEventAndIsNotBreached()
        {
            //Arrange
            var @event = new ClockStartingEvent
            {
                Code = EventCode.ReferralReceived,
                EventDate = new DateTime(2014, 8, 10)
            };

            //Act
            var result = @event.EventStatus;

            //Assert
            Assert.AreEqual(EventStatus.Success, result);
        }

        [TestMethod]
        public void EventStatus_ReturnCorrectEventStatus_WhenEventTypeIsClockStartingEventAndIsBreached()
        {
            //Arrange
            var @event = new ClockStartingEvent
            {
                Code = EventCode.InpatientTCIAgreed,
                EventDate = new DateTime(2014, 8, 10),
                TargetDate = new DateTime(2014, 8, 9)
            };

            //Act
            var result = @event.EventStatus;

            //Assert
            Assert.AreEqual(EventStatus.Breached, result);
        }

        [TestMethod]
        public void EventStatus_ReturnCorrectEventStatus_WhenEventTypeIsClockTickingEventAndIsNotBreached()
        {
            //Arrange
            var @event = new ClockTickingEvent
            {
                Code = EventCode.ReferralReceived,
                EventDate = new DateTime(2014, 8, 10)
            };

            //Act
            var result = @event.EventStatus;

            //Assert
            Assert.AreEqual(EventStatus.Success, result);
        }

        [TestMethod]
        public void EventStatus_ReturnCorrectEventStatus_WhenEventTypeIsClockTickingEventAndIsBreached()
        {
            //Arrange
            var @event = new ClockTickingEvent
            {
                Code = EventCode.InpatientTCIAgreed,
                EventDate = new DateTime(2014, 8, 10),
                TargetDate = new DateTime(2014, 8, 9)
            };

            //Act
            var result = @event.EventStatus;

            //Assert
            Assert.AreEqual(EventStatus.Breached, result);
        }

        [TestMethod]
        public void BreachStatus_ReturnCorrectBreachStatus_WhenEventStatusIsBreached()
        {
            //Arrange
            var @event = new ClockTickingEvent
            {
                Code = EventCode.InpatientTCIAgreed,
                EventDate = new DateTime(2014, 8, 10),
                TargetDate = new DateTime(2014, 8, 9)
            };

            //Act
            var result = @event.BreachStatus;

            //Assert
            Assert.AreEqual(EventBreachStatus.Breached, result);
        }

        [TestMethod]
        public void BreachStatus_ReturnCorrectBreachStatus_WhenEventStatusIsNotBreached()
        {
            //Arrange
            var @event = new ClockTickingEvent
            {
                Code = EventCode.InpatientTCIAgreed,
                EventDate = new DateTime(2014, 8, 10),
                TargetDate = new DateTime(2014, 8, 11)
            };

            //Act
            var result = @event.BreachStatus;

            //Assert
            Assert.AreEqual(EventBreachStatus.Success, result);
        }

        [TestMethod]
        public void Validate_RaisesValidationFailedEvent_WhenClockPausesAndEventOtherThanInpatientTciAgreed()
        {
            //Arrange
            var @event = new ClockPausingEvent { Code = EventCode.OutpatientDischarged, Period = new RTT18WeekPeriod { Name = "period 1", Pathway = new Pathway { PPINumber = "ppi" } } };
            Error eventRaised = null;

            @event.ValidationFailed += delegate { eventRaised = new Error(); };

            //Act
            @event.Validate();

            //Assert
            Assert.IsNotNull(eventRaised);
        }

        [TestMethod]
        public void Validate_DoesNotRaiseValidationFailedEvent_WhenClockPausesAndEventInpatientTciAgreed()
        {
            //Arrange
            var @event = new ClockPausingEvent { Code = EventCode.InpatientTCIOffer, Period = new RTT18WeekPeriod{Name = "period 1", Pathway = new Pathway{PPINumber = "ppi"}}};
            Error eventRaised = null;

            @event.ValidationFailed += delegate { eventRaised = new Error(); };

            //Act
            @event.Validate();

            //Assert
            Assert.IsNull(eventRaised);
        }

        [TestMethod]
        public void Validate_RaisesValidationFailedEvent_WhenClockStopsAndEventOtherThanOutpatientDischarged()
        {
            //Arrange
            var @event = new ClockStoppingEvent { Code = EventCode.InpatientAdmitted, Period = new RTT18WeekPeriod { Name = "period 1", Pathway = new Pathway { PPINumber = "ppi" } } };
            Error eventRaised = null;

            @event.ValidationFailed += delegate { eventRaised = new Error(); };

            //Act
            @event.Validate();

            //Assert
            Assert.IsNotNull(eventRaised);
        }

        [TestMethod]
        public void Validate_DoesNotRaiseValidationFailedEvent_WhenClockStopsAndEventOutpatientDischarged()
        {
            //Arrange
            var @event = new ClockStoppingEvent { Code = EventCode.OutpatientDischarged, Period = new RTT18WeekPeriod { Name = "period 1", Pathway = new Pathway { PPINumber = "ppi" } } };
            Error eventRaised = null;

            @event.ValidationFailed += delegate { eventRaised = new Error(); };

            //Act
            @event.Validate();

            //Assert
            Assert.IsNull(eventRaised);
        }

        [TestMethod]
        public void Validate_RaisesValidationFailedEvent_WhenClockStopsAndEventOtherThanInpatientDischarged()
        {
            //Arrange
            var @event = new ClockStoppingEvent { Code = EventCode.PatientAddedToInpatientWaitingList, Period = new RTT18WeekPeriod { Name = "period 1", Pathway = new Pathway { PPINumber = "ppi" } } };
            var eventRaised = false;

            @event.ValidationFailed += delegate { eventRaised = true; };

            //Act
            @event.Validate();

            //Assert
            Assert.IsTrue(eventRaised);
        }

        [TestMethod]
        public void Validate_DoesNotRaiseValidationFailedEvent_WhenClockStopsAndEventInpatientDischarged()
        {
            //Arrange
            var @event = new ClockStoppingEvent { Code = EventCode.InpatientDischarged, Period = new RTT18WeekPeriod { Name = "period 1", Pathway = new Pathway { PPINumber = "ppi" } } };
            var eventRaised = false;

            @event.ValidationFailed += delegate { eventRaised = true; };

            //Act
            @event.Validate();

            //Assert
            Assert.IsFalse(eventRaised);
        }

        [TestMethod]
        public void Validate_RaisesValidationFailedEvent_WhenClockStopsAndEventOtherThanPathwayCompletion()
        {
            //Arrange
            var @event = new ClockStoppingEvent { Code = EventCode.DiagnosticsOrderPlaced, Period = new RTT18WeekPeriod{Name = "period 1", Pathway = new Pathway{PPINumber = "ppi"}}};
            Error eventRaised = null;

            @event.ValidationFailed += delegate { eventRaised = new Error(); };

            //Act
            @event.Validate();

            //Assert
            Assert.IsNotNull(eventRaised);
        }

        [TestMethod]
        public void Validate_DoesNotRaiseValidationFailedEvent_WhenClockStopsAndEventPathwayCompletion()
        {
            //Arrange
            var @event = new ClockStoppingEvent { Code = EventCode.PathwayCompletion, Period = new RTT18WeekPeriod { Name = "period 1", Pathway = new Pathway { PPINumber = "ppi" } } };
            var eventRaised = false;

            @event.ValidationFailed += delegate { eventRaised = true; };

            //Act
            @event.Validate();

            //Assert
            Assert.IsFalse(eventRaised);
        }

        [TestMethod]
        public void Validate_RaisesValidationFailedEvent_WhenCancerPeriodAndEventPausesPeriod()
        {
            // Arrange
            var @event = new ClockPausingEvent { Code = EventCode.InpatientTCIOffer, EventDate = new DateTime(2014, 12, 26), Period = new CancerPeriod{ Name = "period 1", Pathway = new Pathway { PPINumber = "ppi" } } };
            var eventRaised = false;

            @event.ValidationFailed += delegate { eventRaised = true; };

            // Act
            @event.Validate();

            // Assert
            Assert.IsTrue(eventRaised);
        }
    }
}
