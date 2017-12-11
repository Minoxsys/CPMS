using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CPMS.Domain.Tests
{
    [TestClass]
    public class ClockStartingCompletedEventTests
    {
        private ClockStartingCompletedEvent _clockStartingCompletedEvent;

        [TestInitialize]
        public void PerTestSetup()
        {
            _clockStartingCompletedEvent = new ClockStartingCompletedEvent { EventDate = new DateTime(2000, 12, 25) };
        }

        [TestMethod]
        public void GetDaysSpentAt_ThrowsException_WhenReferenceDateIsLessThanEventDate()
        {
            // Arrange
            bool exceptionThrown = false;

            // Act
            try
            {
                _clockStartingCompletedEvent.GetDaysSpentAt(new DateTime(2000, 11, 25));
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
            var result = _clockStartingCompletedEvent.GetDaysSpentAt(new DateTime(2000, 12, 27));

            // Assert
            Assert.AreEqual(2, result);
        }

        [TestMethod]
        public void Validate_RaisesValidationFailedEvent_WhenPatientCancelsForTheSecondTimeAndPeriodIsNotStopped()
        {
            // Arrange
            var period = new RTT18WeekPeriod { Pathway = new Pathway { Patient = new Patient { DateOfBirth = new DateTime(1980, 2, 3) } } };
            var completedEvent = new ClockStartingCompletedEvent { Name = new EventName { Code = EventCode.PatientCancelEvent }, EventDate = new DateTime(2001, 1, 8), Period = period };
            period.Add(new ClockStartingCompletedEvent { Name = new EventName { Code = EventCode.PatientCancelEvent }, EventDate = new DateTime(2001, 3, 8), Period = period });

            var eventRaised = false;
            completedEvent.ValidationFailed += delegate { eventRaised = true; };

            // Act
            completedEvent.Validate();

            // Assert
            Assert.IsTrue(eventRaised);
        }

        [TestMethod]
        public void Validate_RaisesValidationFailedEvent_WhenPatientIsAChildAndDoesNotAttendForTheSecondTimeAndPeriodIsNotStopped()
        {
            // Arrange
            var period = new RTT18WeekPeriod { Pathway = new Pathway { Patient = new Patient { DateOfBirth = new DateTime(1990, 3, 5) } } };
            var completedEvent = new ClockStartingCompletedEvent { Name = new EventName { Code = EventCode.DidNotAttend }, EventDate = new DateTime(2001, 1, 8), Period = period };
            period.Add(new ClockTickingCompletedEvent { Name = new EventName { Code = EventCode.DidNotAttend }, EventDate = new DateTime(2001, 1, 9) });

            var eventRaised = false;
            completedEvent.ValidationFailed += delegate { eventRaised = true; };

            // Act
            completedEvent.Validate();

            // Assert
            Assert.IsTrue(eventRaised);
        }

        [TestMethod]
        public void Validate_RaisesValidationFailedEvent_WhenPatientDoesNotAttendForTheSecondTimeAndOnAPeriodWhereIsAbleToNotAttendFirstEventAndPeriodIsNotStopped()
        {
            // Arrange
            var period = new CancerPeriod { Pathway = new Pathway { Patient = new Patient { DateOfBirth = new DateTime(1990, 3, 5) } } };
            var completedEvent = new ClockStartingCompletedEvent { Name = new EventName { Code = EventCode.DidNotAttend }, EventDate = new DateTime(2001, 1, 8), Period = period };
            period.Add(new ClockStartingCompletedEvent { Name = new EventName { Code = EventCode.DidNotAttend }, EventDate = new DateTime(2001, 1, 9) });

            var eventRaised = false;
            completedEvent.ValidationFailed += delegate { eventRaised = true; };

            // Act
            completedEvent.Validate();

            // Assert
            Assert.IsTrue(eventRaised);
        }

        [TestMethod]
        public void Validate_RaisesValidationFailedEvent_WhenPatientIsNotAChildAndDoesNotAttendAndPeriodIsNotStopped()
        {
            // Arrange
            var period = new RTT18WeekPeriod { Pathway = new Pathway { Patient = new Patient { DateOfBirth = new DateTime(1980, 3, 5) } } };
            var completedEvent = new ClockStartingCompletedEvent { Name = new EventName { Code = EventCode.DidNotAttend }, EventDate = new DateTime(2001, 1, 8), Period = period };

            var eventRaised = false;
            completedEvent.ValidationFailed += delegate { eventRaised = true; };

            // Act
            completedEvent.Validate();

            // Assert
            Assert.IsTrue(eventRaised);
        }

        [TestMethod]
        public void Validate_RaisesValidationFailedEvent_WhenPatientIsNotOnAPeriodWhereIsAbleToNotAttendFirstEventAndDoesNotAttendAndPeriodIsNotStopped()
        {
            // Arrange
            var period = new RTT18WeekPeriod { Pathway = new Pathway { Patient = new Patient { DateOfBirth = new DateTime(1980, 3, 5) } } };
            var completedEvent = new ClockStartingCompletedEvent { Name = new EventName { Code = EventCode.DidNotAttend }, EventDate = new DateTime(2001, 1, 8), Period = period };

            var eventRaised = false;
            completedEvent.ValidationFailed += delegate { eventRaised = true; };

            // Act
            completedEvent.Validate();

            // Assert
            Assert.IsTrue(eventRaised);
        }
    }
}
