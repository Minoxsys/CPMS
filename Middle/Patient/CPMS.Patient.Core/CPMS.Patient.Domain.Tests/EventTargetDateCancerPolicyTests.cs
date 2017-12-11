//using System;
//using Microsoft.VisualStudio.TestTools.UnitTesting;

//namespace CPMS.Patient.Domain.Tests
//{
//    [TestClass]
//    public class EventTargetDateCancerPolicyTests
//    {
//        [TestMethod]
//        public void ApplyTo_DoesntOverwriteTargetDate_WhenEventIsNotCancer()
//        {
//            // Arrange
//            var clockTickingEvent = new ClockTickingEvent {TargetDate = new DateTime(2000, 12, 1)};
//            var sut = new EventTargetDateCancerPolicy();

//            // Act
//            sut.ApplyTo(clockTickingEvent);

//            // Assert
//            Assert.AreEqual(new DateTime(2000, 12, 1), clockTickingEvent.TargetDate);
//        }

//        [TestMethod]
//        public void ApplyTo_CorrectlyOverwritesTargetDate_WhenEventCodeIsDiagnosticTestResultAvailable()
//        {
//            // Arrange
//            var period = new RTT18WeekPeriod();
//            period.Add(new ClockStartingEvent
//            {
//                EventDate = new DateTime(2000, 6, 1),
//                Code = EventCode.ReferralReceived
//            });

//            var clockTickingEvent = new ClockTickingEvent
//            {
//                Cancer = true,
//                TargetDate = new DateTime(2000, 12, 1),
//                Code = EventCode.DiagnosticsTestResultAvailable,
//                Period = period
//            };
//            var sut = new EventTargetDateCancerPolicy();

//            // Act
//            sut.ApplyTo(clockTickingEvent);

//            // Assert
//            Assert.AreEqual(new DateTime(2000, 6, 1).AddDays(Constants.DignosticTestResultAvailableTargetNumberOfDaysForCancer), clockTickingEvent.TargetDate);
//        }

//        [TestMethod]
//        public void ApplyTo_CorrectlyOverwritesTargetDate_WhenEventCodeIsOutpatientFirstAppointmentAttended()
//        {
//            // Arrange
//            var period = new RTT18WeekPeriod();
//            period.Add(new ClockStartingEvent
//            {
//                EventDate = new DateTime(2000, 6, 1),
//                Code = EventCode.ReferralReceived
//            });
//            var clockTickingEvent = new ClockTickingEvent
//            {
//                Cancer = true,
//                TargetDate = new DateTime(2000, 12, 1),
//                Code = EventCode.AttendedOutpatientFirstAppointment,
//                Period = period
//            };
//            var sut = new EventTargetDateCancerPolicy();

//            // Act
//            sut.ApplyTo(clockTickingEvent);

//            // Assert
//            Assert.AreEqual(new DateTime(2000, 6, 1).AddDays(Constants.OutpatientFirstAppointmentAttendedTargetNumberOfDaysForCancer), clockTickingEvent.TargetDate);
//        }
//    }
//}
