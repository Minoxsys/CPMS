using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CPMS.Domain.Tests
{
    [TestClass]
    public class EventMilestoneDateReferenceForTargetCancerPolicyTests
    {
        [TestMethod]
        public void ApplyTo_DoesntOverwriteDateReferenceForTarget_WhenEventIsNotCancer()
        {
            // Arrange
            var eventMilestone = new EventMilestone
            {
                DateReferenceForTarget = new DateTime(2000, 12, 1),
                CompletedEvent = new ClockTickingCompletedEvent()
            };

            var sut = new EventMilestoneDateReferenceForTargetCancerPolicy();

            // Act
            sut.ApplyTo(eventMilestone);

            // Assert
            Assert.AreEqual(new DateTime(2000, 12, 1), eventMilestone.DateReferenceForTarget);
        }

        [TestMethod]
        public void ApplyTo_CorrectlyOverwritesDateReferenceForTarget_WhenEventCodeIsDiagnosticTestResultAvailable()
        {
            // Arrange
            var period = new CancerPeriod();
            period.Add(new ClockStartingCompletedEvent
            {
                EventDate = new DateTime(2000, 6, 1),
                Name = new EventName { Code = EventCode.ReferralReceived }
            });
            var eventMilestone = new EventMilestone
            {
                DateReferenceForTarget = new DateTime(2000, 12, 1),
                Name = new EventName { Code = EventCode.DiagnosticsTestResultAvailable },
                CompletedEvent = new ClockTickingCompletedEvent
                {
                    Cancer = true,
                    Period = period
                }
            };

            var sut = new EventMilestoneDateReferenceForTargetCancerPolicy();

            // Act
            sut.ApplyTo(eventMilestone);

            // Assert
            Assert.AreEqual(new DateTime(2000, 6, 1), eventMilestone.DateReferenceForTarget);
        }

        [TestMethod]
        public void ApplyTo_CorrectlyOverwritesDateReferenceForTarget_WhenEventCodeIsOutpatientFirstAppointmentAttended()
        {
            // Arrange
            var period = new CancerPeriod();
            period.Add(new ClockStartingCompletedEvent
            {
                EventDate = new DateTime(2000, 6, 1),
                Name = new EventName { Code = EventCode.ReferralReceived }
            });
            var eventMilestone = new EventMilestone
            {
                DateReferenceForTarget = new DateTime(2000, 12, 1),
               Name = new EventName { Code = EventCode.AttendedOutpatientFirstAppointment },
                CompletedEvent = new ClockTickingCompletedEvent
                {
                    Cancer = true,
                    Period = period
                }
            };

            var sut = new EventMilestoneDateReferenceForTargetCancerPolicy();

            // Act
            sut.ApplyTo(eventMilestone);

            // Assert
            Assert.AreEqual(new DateTime(2000, 6, 1), eventMilestone.DateReferenceForTarget);
        }
    }
}
