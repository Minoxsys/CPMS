using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CPMS.Domain.Tests
{
    [TestClass]
    public class EventMilestoneTargetNumberOfDaysCancerPolicyTests
    {
        [TestMethod]
        public void ApplyTo_DoesntOverwriteTargetNumberOfDays_WhenEventIsNotCancer()
        {
            // Arrange
            var eventMilestone = new EventMilestone
            {
                TargetNumberOfDays = 5,
                CompletedEvent = new ClockTickingCompletedEvent()
            };

            var sut = new EventMilestoneTargetNumberOfDaysCancerPolicy();

            // Act
            sut.ApplyTo(eventMilestone);

            // Assert
            Assert.AreEqual(5, eventMilestone.TargetNumberOfDays);
        }

        [TestMethod]
        public void ApplyTo_CorrectlyOverwritesTargetNumberOfDays_WhenEventCodeIsDiagnosticTestResultAvailable()
        {
            // Arrange
            var eventMilestone = new EventMilestone
            {
                TargetNumberOfDays = 5,
                Name = new EventName { Code = EventCode.DiagnosticsTestResultAvailable },
                CompletedEvent = new ClockTickingCompletedEvent
                {
                    Cancer = true
                }
            };

            var sut = new EventMilestoneTargetNumberOfDaysCancerPolicy();

            // Act
            sut.ApplyTo(eventMilestone);

            // Assert
            Assert.AreEqual(Constants.DignosticTestResultAvailableTargetNumberOfDaysForCancer, eventMilestone.TargetNumberOfDays);
        }

        [TestMethod]
        public void ApplyTo_CorrectlyOverwritesTargetNumberOfDays_WhenEventCodeIsOutpatientFirstAppointmentAttended()
        {
            // Arrange
            var eventMilestone = new EventMilestone
            {
                TargetNumberOfDays = 5,
                Name = new EventName { Code = EventCode.AttendedOutpatientFirstAppointment },
                CompletedEvent = new ClockTickingCompletedEvent
                {
                    Cancer = true
                }
            };

            var sut = new EventMilestoneTargetNumberOfDaysCancerPolicy();

            // Act
            sut.ApplyTo(eventMilestone);

            // Assert
            Assert.AreEqual(Constants.OutpatientFirstAppointmentAttendedTargetNumberOfDaysForCancer, eventMilestone.TargetNumberOfDays);
        }
    }
}
