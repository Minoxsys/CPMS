using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CPMS.Patient.Domain.Tests
{
    [TestClass]
    public class ClockStoppingEventTests
    {
        private ClockStoppingEvent clockStoppingEvent;

        [TestInitialize]
        public void PerTestSetup()
        {
            clockStoppingEvent = new ClockStoppingEvent { EventDate = new DateTime(2000, 12, 25) };
        }

        [TestMethod]
        public void GetDaysSpentAt_ThrowsException_WhenReferenceDateIsLessThanEventDate()
        {
            // Arrange
            bool exceptionThrown = false;

            // Act
            try
            {
                clockStoppingEvent.GetDaysSpentAt(new DateTime(2000, 11, 25));
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
            var result = clockStoppingEvent.GetDaysSpentAt(new DateTime(2000, 12, 27));

            // Assert
            Assert.AreEqual(0, result);
        }

        [TestMethod]
        public void GetDaysSpentAt_Returns0_WhenReferenceDateAndEventDateAreInTheSameDay()
        {
            // Arrange
            // Act
            var result = clockStoppingEvent.GetDaysSpentAt(new DateTime(2000, 12, 25));

            // Assert
            Assert.AreEqual(0, result);
        }

        [TestMethod]
        public void Validate_DoesNotRaiseValidationFailedEvent_WhenEventCodeIsOutpatientDischarged()
        {
            // Arrange
            var eventRaised = false;
            clockStoppingEvent.Code = EventCode.OutpatientDischarged;
            clockStoppingEvent.ValidationFailed += delegate { eventRaised = true; };

            // Act
            clockStoppingEvent.Validate();

            // Assert
            Assert.IsFalse(eventRaised);
        }


        [TestMethod]
        public void Validate_DoesNotRaiseValidationFailedEvent_WhenEventCodeIsInpatientDischarged()
        {
            // Arrange
            var eventRaised = false;
            clockStoppingEvent.Code = EventCode.InpatientDischarged;
            clockStoppingEvent.ValidationFailed += delegate { eventRaised = true; };

            // Act
            clockStoppingEvent.Validate();

            // Assert
            Assert.IsFalse(eventRaised);
        }


        [TestMethod]
        public void Validate_DoesNotRaiseValidationFailedEvent_WhenEventCodeIsPathwayCompletion()
        {
            // Arrange
            var eventRaised = false;
            clockStoppingEvent.Code = EventCode.PathwayCompletion;
            clockStoppingEvent.ValidationFailed += delegate { eventRaised = true; };

            // Act
            clockStoppingEvent.Validate();

            // Assert
            Assert.IsFalse(eventRaised);
        }


        [TestMethod]
        public void Validate_DoesNotRaiseValidationFailedEvent_WhenEventCodeIsPatientCancelEvent()
        {
            // Arrange
            var eventRaised = false;
            clockStoppingEvent.Code = EventCode.PatientCancelEvent;
            clockStoppingEvent.ValidationFailed += delegate { eventRaised = true; };

            // Act
            clockStoppingEvent.Validate();

            // Assert
            Assert.IsFalse(eventRaised);
        }


        [TestMethod]
        public void Validate_DoesNotRaiseValidationFailedEvent_WhenEventCodeIsDidNotAttend()
        {
            // Arrange
            var eventRaised = false;
            clockStoppingEvent.Code = EventCode.DidNotAttend;
            clockStoppingEvent.ValidationFailed += delegate { eventRaised = true; };

            // Act
            clockStoppingEvent.Validate();

            // Assert
            Assert.IsFalse(eventRaised);
        }
    }
}
