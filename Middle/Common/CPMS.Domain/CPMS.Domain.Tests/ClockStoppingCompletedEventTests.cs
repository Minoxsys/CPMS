using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CPMS.Domain.Tests
{
    [TestClass]
    public class ClockStoppingCompletedEventTests
    {
        private ClockStoppingCompletedEvent _clockStoppingCompletedEvent;

        [TestInitialize]
        public void PerTestSetup()
        {
            _clockStoppingCompletedEvent = new ClockStoppingCompletedEvent { EventDate = new DateTime(2000, 12, 25) };
        }

        [TestMethod]
        public void GetDaysSpentAt_ThrowsException_WhenReferenceDateIsLessThanEventDate()
        {
            // Arrange
            bool exceptionThrown = false;

            // Act
            try
            {
                _clockStoppingCompletedEvent.GetDaysSpentAt(new DateTime(2000, 11, 25));
            }
            catch
            {
                exceptionThrown = true;
            }

            // Assert
            Assert.IsTrue(exceptionThrown);
        }

        [TestMethod]
        public void GetDaysSpentAt_Returns0_WhenReferenceDateIsGreaterThanEventDate()
        {
            // Arrange
            // Act
            var result = _clockStoppingCompletedEvent.GetDaysSpentAt(new DateTime(2000, 12, 27));

            // Assert
            Assert.AreEqual(0, result);
        }

        [TestMethod]
        public void GetDaysSpentAt_Returns0_WhenReferenceDateAndEventDateAreInTheSameDay()
        {
            // Arrange
            // Act
            var result = _clockStoppingCompletedEvent.GetDaysSpentAt(new DateTime(2000, 12, 25));

            // Assert
            Assert.AreEqual(0, result);
        }

        [TestMethod]
        public void Validate_DoesNotRaiseValidationFailedEvent_WhenEventCodeIsOutpatientDischarged()
        {
            // Arrange
            var eventRaised = false;
            _clockStoppingCompletedEvent.Name = new EventName { Code = EventCode.OutpatientDischarged, Description = "Outpatient Discharged" };
            _clockStoppingCompletedEvent.ValidationFailed += delegate { eventRaised = true; };

            // Act
            _clockStoppingCompletedEvent.Validate();

            // Assert
            Assert.IsFalse(eventRaised);
        }

        [TestMethod]
        public void Validate_DoesNotRaiseValidationFailedEvent_WhenEventCodeIsInpatientDischarged()
        {
            // Arrange
            var eventRaised = false;
            _clockStoppingCompletedEvent.Name = new EventName { Code = EventCode.InpatientDischarged, Description = "Inpatient Discharged" };
            _clockStoppingCompletedEvent.ValidationFailed += delegate { eventRaised = true; };

            // Act
            _clockStoppingCompletedEvent.Validate();

            // Assert
            Assert.IsFalse(eventRaised);
        }

        [TestMethod]
        public void Validate_DoesNotRaiseValidationFailedEvent_WhenEventCodeIsPathwayCompletion()
        {
            // Arrange
            var eventRaised = false;
            _clockStoppingCompletedEvent.Name = new EventName { Code = EventCode.PathwayCompletion, Description = "Pathway Completion" };
            _clockStoppingCompletedEvent.ValidationFailed += delegate { eventRaised = true; };

            // Act
            _clockStoppingCompletedEvent.Validate();

            // Assert
            Assert.IsFalse(eventRaised);
        }

        [TestMethod]
        public void Validate_DoesNotRaiseValidationFailedEvent_WhenEventCodeIsPatientCancelEvent()
        {
            // Arrange
            var eventRaised = false;
            _clockStoppingCompletedEvent.Name = new EventName { Code = EventCode.PatientCancelEvent, Description = "Patient Cancel Event" };
            _clockStoppingCompletedEvent.ValidationFailed += delegate { eventRaised = true; };

            // Act
            _clockStoppingCompletedEvent.Validate();

            // Assert
            Assert.IsFalse(eventRaised);
        }

        [TestMethod]
        public void Validate_DoesNotRaiseValidationFailedEvent_WhenEventCodeIsDidNotAttend()
        {
            // Arrange
            var eventRaised = false;
            _clockStoppingCompletedEvent.Name = new EventName { Code = EventCode.DidNotAttend, Description = "Did Not Attend" };
            _clockStoppingCompletedEvent.ValidationFailed += delegate { eventRaised = true; };

            // Act
            _clockStoppingCompletedEvent.Validate();

            // Assert
            Assert.IsFalse(eventRaised);
        }
    }
}
