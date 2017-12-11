﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CPMS.Patient.Domain.Tests
{
    [TestClass]
    public class ClockTickingEventTests
    {
        private ClockTickingEvent _clockTickingEvent;

        [TestInitialize]
        public void PerTestSetup()
        {
            _clockTickingEvent = new ClockTickingEvent { EventDate = new DateTime(2000, 12, 25) };
        }

        [TestMethod]
        public void GetDaysSpentAt_ThrowsException_WhenReferenceDateIsLessThanEventDate()
        {
            // Arrange
            bool exceptionThrown = false;

            // Act
            try
            {
                _clockTickingEvent.GetDaysSpentAt(new DateTime(2000, 11, 25));
            }
            catch
            {
                exceptionThrown = true;
            }

            // Assert
            Assert.IsTrue(exceptionThrown);
        }

        [TestMethod]
        public void GetDaysSpentAt_CorrectlyReturnsDifference_WhenReferenceDateIsGreaterThanEventDate()
        {
            // Arrange
            // Act
            var result = _clockTickingEvent.GetDaysSpentAt(new DateTime(2000, 12, 27));

            // Assert
            Assert.AreEqual(2, result);
        }

        [TestMethod]
        public void GetDaysSpentAt_Returns0_WhenReferenceDateAndEventDateAreInTheSameDay()
        {
            // Arrange
            // Act
            var result = _clockTickingEvent.GetDaysSpentAt(new DateTime(2000, 12, 25));

            // Assert
            Assert.AreEqual(0, result);
        }

        [TestMethod]
        public void Validate_RaisesValidationFailedEvent_WhenPatientCancelsForTheSecondTimeAndPeriodIsNotStopped()
        {
            // Arrange
            var period = new RTT18WeekPeriod { Pathway = new Pathway { Patient = new Patient { DateOfBirth = new DateTime(1980, 2, 3) } } };
            var @event = new ClockTickingEvent { Code = EventCode.PatientCancelEvent, EventDate = new DateTime(2001, 1, 8), Period = period };
            period.Add(new ClockTickingEvent { Code = EventCode.PatientCancelEvent, EventDate = new DateTime(2001, 3, 8), Period = period });

            var eventRaised = false;
            @event.ValidationFailed += delegate { eventRaised = true; };

            // Act
            @event.Validate();

            // Assert
            Assert.IsTrue(eventRaised);
        }

        [TestMethod]
        public void Validate_RaisesValidationFailedEvent_WhenPatientIsAChildAndDoesNotAttendForTheSecondTimeAndPeriodIsNotStopped()
        {
            // Arrange
            var period = new RTT18WeekPeriod { Pathway = new Pathway { Patient = new Patient { DateOfBirth = new DateTime(1990, 3, 5) } } };
            var @event = new ClockTickingEvent { Code = EventCode.DidNotAttend, EventDate = new DateTime(2001, 1, 8), Period = period };
            period.Add(new ClockTickingEvent { Code = EventCode.DidNotAttend, EventDate = new DateTime(2001, 1, 9) });

            var eventRaised = false;
            @event.ValidationFailed += delegate { eventRaised = true; };

            // Act
            @event.Validate();

            // Assert
            Assert.IsTrue(eventRaised);
        }

        [TestMethod]
        public void Validate_RaisesValidationFailedEvent_WhenPatientDoesNotAttendForTheSecondTimeAndOnAPeriodWhereIsAbleToNotAttendFirstEventAndPeriodIsNotStopped()
        {
            // Arrange
            var period = new CancerPeriod { Pathway = new Pathway { Patient = new Patient { DateOfBirth = new DateTime(1990, 3, 5) } } };
            var @event = new ClockTickingEvent { Code = EventCode.DidNotAttend, EventDate = new DateTime(2001, 1, 8), Period = period };
            period.Add(new ClockTickingEvent { Code = EventCode.DidNotAttend, EventDate = new DateTime(2001, 1, 9) });

            var eventRaised = false;
            @event.ValidationFailed += delegate { eventRaised = true; };

            // Act
            @event.Validate();

            // Assert
            Assert.IsTrue(eventRaised);
        }

        [TestMethod]
        public void Validate_RaisesValidationFailedEvent_WhenPatientIsNotAChildAndDoesNotAttendAndPeriodIsNotStopped()
        {
            // Arrange
            var period = new RTT18WeekPeriod { Pathway = new Pathway { Patient = new Patient { DateOfBirth = new DateTime(1980, 3, 5) } } };
            var @event = new ClockTickingEvent { Code = EventCode.DidNotAttend, EventDate = new DateTime(2001, 1, 8), Period = period };

            var eventRaised = false;
            @event.ValidationFailed += delegate { eventRaised = true; };

            // Act
            @event.Validate();

            // Assert
            Assert.IsTrue(eventRaised);
        }

        [TestMethod]
        public void Validate_RaisesValidationFailedEvent_WhenPatientIsNotOnAPeriodWhereIsAbleToNotAttendFirstEventAndDoesNotAttendAndPeriodIsNotStopped()
        {
            // Arrange
            var period = new RTT18WeekPeriod { Pathway = new Pathway { Patient = new Patient { DateOfBirth = new DateTime(1980, 3, 5) } } };
            var @event = new ClockTickingEvent { Code = EventCode.DidNotAttend, EventDate = new DateTime(2001, 1, 8), Period = period };

            var eventRaised = false;
            @event.ValidationFailed += delegate { eventRaised = true; };

            // Act
            @event.Validate();

            // Assert
            Assert.IsTrue(eventRaised);
        }
    }
}
