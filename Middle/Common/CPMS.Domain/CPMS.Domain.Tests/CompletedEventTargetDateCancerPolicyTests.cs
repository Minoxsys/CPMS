using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CPMS.Domain.Tests
{
    [TestClass]
    public class CompletedEventTargetDateCancerPolicyTests
    {
        [TestMethod]
        public void ApplyTo_DoesntOverwriteTargetDate_WhenEventIsNotCancer()
        {
            // Arrange
            var clockTickingEvent = new ClockTickingCompletedEvent {TargetDate = new DateTime(2000, 12, 1)};
            var sut = new CompletedEventTargetDateCancerPolicy();

            // Act
            sut.ApplyTo(clockTickingEvent);

            // Assert
            Assert.AreEqual(new DateTime(2000, 12, 1), clockTickingEvent.TargetDate);
        }

        [TestMethod]
        public void ApplyTo_CorrectlyOverwritesTargetDate_WhenEventCodeIsDiagnosticTestResultAvailable()
        {
            // Arrange
            var period = new RTT18WeekPeriod();
            period.Add(new ClockStartingCompletedEvent
            {
                EventDate = new DateTime(2000, 6, 1),
                Name = new EventName { Code = EventCode.ReferralReceived }
            });

            var clockTickingEvent = new ClockTickingCompletedEvent
            {
                Cancer = true,
                TargetDate = new DateTime(2000, 12, 1),
                Name = new EventName { Code = EventCode.DiagnosticsTestResultAvailable },
                Period = period
            };
            var sut = new CompletedEventTargetDateCancerPolicy();

            // Act
            sut.ApplyTo(clockTickingEvent);

            // Assert
            Assert.AreEqual(new DateTime(2000, 6, 1).AddDays(Constants.DignosticTestResultAvailableTargetNumberOfDaysForCancer), clockTickingEvent.TargetDate);
        }

        [TestMethod]
        public void ApplyTo_CorrectlyOverwritesTargetDate_WhenEventCodeIsOutpatientFirstAppointmentAttended()
        {
            // Arrange
            var period = new RTT18WeekPeriod();
            period.Add(new ClockStartingCompletedEvent
            {
                EventDate = new DateTime(2000, 6, 1),
                Name = new EventName { Code = EventCode.ReferralReceived }
            });
            var clockTickingEvent = new ClockTickingCompletedEvent
            {
                Cancer = true,
                TargetDate = new DateTime(2000, 12, 1),
                Name = new EventName { Code = EventCode.AttendedOutpatientFirstAppointment },
                Period = period
            };
            var sut = new CompletedEventTargetDateCancerPolicy();

            // Act
            sut.ApplyTo(clockTickingEvent);

            // Assert
            Assert.AreEqual(new DateTime(2000, 6, 1).AddDays(Constants.OutpatientFirstAppointmentAttendedTargetNumberOfDaysForCancer), clockTickingEvent.TargetDate);
        }
    }
}
