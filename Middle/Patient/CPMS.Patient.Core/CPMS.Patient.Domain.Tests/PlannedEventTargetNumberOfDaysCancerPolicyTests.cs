using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CPMS.Patient.Domain.Tests
{
    [TestClass]
    public class PlannedEventTargetNumberOfDaysCancerPolicyTests
    {
        [TestMethod]
        public void ApplyTo_DoesntOverwriteTargetNumberOfDays_WhenEventIsNotCancer()
        {
            // Arrange
            var plannedEvent = new PlannedEvent
            {
                TargetNumberOfDays = 5,
                Event = new ClockTickingEvent()
            };

            var sut = new PlannedEventTargetNumberOfDaysCancerPolicy();

            // Act
            sut.ApplyTo(plannedEvent);

            // Assert
            Assert.AreEqual(5, plannedEvent.TargetNumberOfDays);
        }

        [TestMethod]
        public void ApplyTo_CorrectlyOverwritesTargetNumberOfDays_WhenEventCodeIsDiagnosticTestResultAvailable()
        {
            // Arrange
            var plannedEvent = new PlannedEvent
            {
                TargetNumberOfDays = 5,
                Code = EventCode.DiagnosticsTestResultAvailable,
                Event = new ClockTickingEvent
                {
                    Cancer = true
                }
            };

            var sut = new PlannedEventTargetNumberOfDaysCancerPolicy();

            // Act
            sut.ApplyTo(plannedEvent);

            // Assert
            Assert.AreEqual(Constants.DignosticTestResultAvailableTargetNumberOfDaysForCancer, plannedEvent.TargetNumberOfDays);
        }

        [TestMethod]
        public void ApplyTo_CorrectlyOverwritesTargetNumberOfDays_WhenEventCodeIsOutpatientFirstAppointmentAttended()
        {
            // Arrange
            var plannedEvent = new PlannedEvent
            {
                TargetNumberOfDays = 5,
                Code = EventCode.AttendedOutpatientFirstAppointment,
                Event = new ClockTickingEvent
                {
                    Cancer = true
                }
            };

            var sut = new PlannedEventTargetNumberOfDaysCancerPolicy();

            // Act
            sut.ApplyTo(plannedEvent);

            // Assert
            Assert.AreEqual(Constants.OutpatientFirstAppointmentAttendedTargetNumberOfDaysForCancer, plannedEvent.TargetNumberOfDays);
        }
    }
}
