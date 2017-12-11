using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CPMS.Patient.Domain.Tests
{
    [TestClass]
    public class PlannedEventDateReferenceForTargetCancerPolicyTests
    {
        [TestMethod]
        public void ApplyTo_DoesntOverwriteDateReferenceForTarget_WhenEventIsNotCancer()
        {
            // Arrange
            var plannedEvent = new PlannedEvent
            {
                DateReferenceForTarget = new DateTime(2000, 12, 1),
                Event = new ClockTickingEvent()
            };

            var sut = new PlannedEventDateReferenceForTargetCancerPolicy();

            // Act
            sut.ApplyTo(plannedEvent);

            // Assert
            Assert.AreEqual(new DateTime(2000, 12, 1), plannedEvent.DateReferenceForTarget);
        }

        [TestMethod]
        public void ApplyTo_CorrectlyOverwritesDateReferenceForTarget_WhenEventCodeIsDiagnosticTestResultAvailable()
        {
            // Arrange
            var period = new CancerPeriod();
            period.Add(new ClockStartingEvent
            {
                EventDate = new DateTime(2000, 6, 1),
                Code = EventCode.ReferralReceived
            });
            var plannedEvent = new PlannedEvent
            {
                DateReferenceForTarget = new DateTime(2000, 12, 1),
                Code = EventCode.DiagnosticsTestResultAvailable,
                Event = new ClockTickingEvent
                {
                    Cancer = true,
                    Period = period
                }
            };

            var sut = new PlannedEventDateReferenceForTargetCancerPolicy();

            // Act
            sut.ApplyTo(plannedEvent);

            // Assert
            Assert.AreEqual(new DateTime(2000, 6, 1), plannedEvent.DateReferenceForTarget);
        }

        [TestMethod]
        public void ApplyTo_CorrectlyOverwritesDateReferenceForTarget_WhenEventCodeIsOutpatientFirstAppointmentAttended()
        {
            // Arrange
            var period = new CancerPeriod();
            period.Add(new ClockStartingEvent
            {
                EventDate = new DateTime(2000, 6, 1),
                Code = EventCode.ReferralReceived
            });
            var plannedEvent = new PlannedEvent
            {
                DateReferenceForTarget = new DateTime(2000, 12, 1),
                Code = EventCode.AttendedOutpatientFirstAppointment,
                Event = new ClockTickingEvent
                {
                    Cancer = true,
                    Period = period
                }
            };

            var sut = new PlannedEventDateReferenceForTargetCancerPolicy();

            // Act
            sut.ApplyTo(plannedEvent);

            // Assert
            Assert.AreEqual(new DateTime(2000, 6, 1), plannedEvent.DateReferenceForTarget);
        }
    }
}
