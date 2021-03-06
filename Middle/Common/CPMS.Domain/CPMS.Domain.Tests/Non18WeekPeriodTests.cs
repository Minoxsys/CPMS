﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace CPMS.Domain.Tests
{
    [TestClass]
    public class Non18WeekPeriodTests
    {
        [TestMethod]
        public void GetDaysSpentAt_Returns0()
        {
            // Arrange
            var non18WeekPeriod = GetNon18WeekPeriod();

            // Act
            var result = non18WeekPeriod.GetDaysSpentAt(It.IsAny<DateTime>());

            // Assert
            Assert.AreEqual(0, result);
        }

        [TestMethod]
        public void GetDaysRemainingAt_Returns0()
        {
            // Arrange
            var non18WeekPeriod = GetNon18WeekPeriod();

            // Act
            var result = non18WeekPeriod.GetDaysRemainingAt(It.IsAny<DateTime>());

            // Assert
            Assert.AreEqual(0, result);
        }

        [TestMethod]
        public void GetNumberOfPausedDays_Returns0()
        {
            // Arrange
            var non18WeekPeriod = GetNon18WeekPeriod();

            // Act
            var result = non18WeekPeriod.GetNumberOfPausedDays(It.IsAny<DateTime>(), It.IsAny<DateTime>());

            // Assert
            Assert.AreEqual(0, result);
        }

        [TestMethod]
        public void GetDelayOrAdvancementDays_Returns0()
        {
            // Arrange
            var non18WeekPeriod = GetNon18WeekPeriod();

            // Act
            var result = non18WeekPeriod.GetDelayOrAdvancementDays(It.IsAny<DateTime>());

            // Assert
            Assert.AreEqual(0, result);
        }

        [TestMethod]
        public void Add_RaisesValidationFailedEvent_WhenCurrentEventHasADateBeforeAnotherEvent()
        {
            // Arrange
            var non18WeekPeriod = GetNon18WeekPeriod();
            var completedEvent = new ClockTickingCompletedEvent { Name = new EventName { Code = EventCode.InpatientPatientAttendancePreOperativeAssessment }, EventDate = new DateTime(2000, 12, 26), Period = non18WeekPeriod };
            
            RuleViolation eventRaised = null;

            non18WeekPeriod.ValidationFailed += delegate { eventRaised = new RuleViolation(); };

            // Act
            non18WeekPeriod.Add(completedEvent);

            // Assert
            Assert.IsNotNull(eventRaised);
        }

        [TestMethod]
        public void MapEventMilestoneToCompletedEvent_RaisesValidationFailedEvent_WhenCurrentEventDoesNotHaveTargetReferenceEvent()
        {
            //Arrange
            var non18WeekPeriod = new Non18WeekPeriod { Pathway = new Pathway { Patient = new Patient { Name = "John Doe", DateOfBirth = new DateTime(1990, 2, 2) }, PPINumber = "ppi" }, Name = "period 1" };
            var completedEvent = new ClockTickingCompletedEvent { Name = new EventName { Code = EventCode.AttendedOutpatientFirstAppointment }, EventDate = new DateTime(2014, 5, 20), Period = non18WeekPeriod };
            non18WeekPeriod.Add(new ClockStartingCompletedEvent { Name = new EventName { Code = EventCode.ReferralReceived }, EventDate = new DateTime(2014, 5, 6) });
            non18WeekPeriod.Add(new ClockTickingCompletedEvent { Name = new EventName { Code = EventCode.ReferralReview }, EventDate = new DateTime(2014, 5, 10) });
            non18WeekPeriod.Add(new ClockTickingCompletedEvent { Name = new EventName { Code = EventCode.BookedOutpatientFirstAppointment }, EventDate = new DateTime(2014, 5, 12) });
            non18WeekPeriod.Add(new ClockTickingCompletedEvent { Name = new EventName { Code = EventCode.OutpatientFirstAppointment }, EventDate = new DateTime(2014, 5, 14) });

            var eventMilestone = new EventMilestone { Name = new EventName { Code = EventCode.OutcomedOutpatientFirstAppointment } };

            RuleViolation eventRaised = null;

            non18WeekPeriod.ValidationFailed += delegate { eventRaised = new RuleViolation(); };

            //Act
            non18WeekPeriod.MapEventMilestoneToCompletedEvent(completedEvent, eventMilestone, new EventName { Code = EventCode.AttendedOutpatientFirstAppointment, Description = "Attended Outpatient First Appointment" });

            //Assert
            Assert.IsNotNull(eventRaised);
        }

        [TestMethod]
        public void MapEventMilestoneToCompletedEvent_DoesNotRaiseValidationFailedEvent_WhenCurrentEventHasTargetReferenceEvent()
        {
            //Arrange
            var non18WeekPeriod = new Non18WeekPeriod { Pathway = new Pathway { Patient = new Patient { Name = "John Doe", DateOfBirth = new DateTime(1990, 2, 2) }, PPINumber = "ppi" }, Name = "period 1" };
            var completedEvent = new ClockTickingCompletedEvent { Name = new EventName { Code = EventCode.AttendedOutpatientFirstAppointment }, EventDate = new DateTime(2014, 5, 20), Period = non18WeekPeriod };
            non18WeekPeriod.Add(new ClockStartingCompletedEvent { Name = new EventName { Code = EventCode.ReferralReceived }, EventDate = new DateTime(2014, 5, 6) });
            non18WeekPeriod.Add(new ClockTickingCompletedEvent { Name = new EventName { Code = EventCode.ReferralReview }, EventDate = new DateTime(2014, 5, 10) });
            non18WeekPeriod.Add(new ClockTickingCompletedEvent { Name = new EventName { Code = EventCode.BookedOutpatientFirstAppointment }, EventDate = new DateTime(2014, 5, 12) });
            non18WeekPeriod.Add(new ClockTickingCompletedEvent { Name = new EventName { Code = EventCode.OutpatientFirstAppointment }, EventDate = new DateTime(2014, 5, 14) });
            non18WeekPeriod.Add(completedEvent);

            var eventMilestone = new EventMilestone { Name = new EventName { Code = EventCode.OutcomedOutpatientFirstAppointment } };

            RuleViolation eventRaised = null;

            non18WeekPeriod.ValidationFailed += delegate { eventRaised = new RuleViolation(); };

            //Act
            non18WeekPeriod.MapEventMilestoneToCompletedEvent(completedEvent, eventMilestone, new EventName { Code = EventCode.AttendedOutpatientFirstAppointment, Description = "Attended Outpatient First Appointment" });

            //Assert
            Assert.IsNull(eventRaised);
        }

        [TestMethod]
        public void GetPauseValidationMessage_ReturnValidationMessageForNon18W()
        {
            // Arrange
            var non18WeekPeriod = GetNon18WeekPeriod();

            // Act
            var result = non18WeekPeriod.GetPauseValidationMessage;

            // Assert
            Assert.IsTrue(!string.IsNullOrEmpty(result));
        }

        [TestMethod]
        public void AbleToNotAttendFirstEvent_ReturnsFalse()
        {
            // Arrange
            var rttPeriod = GetNon18WeekPeriod();

            // Act
            var result = rttPeriod.AbleToNotAttendFirstEvent;

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void ShouldCountForBreaches_ReturnsFalse()
        {
            // Arrange
            var non18WeekPeriod = GetNon18WeekPeriod();

            // Act
            var result = non18WeekPeriod.ShouldCountForBreaches;

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void SecondCancelByPatient_ReturnsTrue_WhenPatientCancelsTheSecondTime()
        {
            // Arrange
            var rttPeriod = new Non18WeekPeriod { Pathway = new Pathway { Patient = new Patient { Name = "John Doe", DateOfBirth = new DateTime(1990, 2, 2) } } };
            rttPeriod.Add(new ClockTickingCompletedEvent { Name = new EventName { Code = EventCode.PatientCancelEvent } });

            // Act
            var result = rttPeriod.IsSecondCancelByPatient(EventCode.PatientCancelEvent);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void IsSecondCancelByPatient_ReturnsTrue_WhenPatientCancelsThreeTimesAndHospitalOnce()
        {
            // Arrange
            var rttPeriod = new Non18WeekPeriod { Pathway = new Pathway { Patient = new Patient { Name = "John Doe", DateOfBirth = new DateTime(1990, 2, 2) } } };
            rttPeriod.Add(new ClockTickingCompletedEvent { Name = new EventName { Code = EventCode.PatientCancelEvent } });
            rttPeriod.Add(new ClockTickingCompletedEvent { Name = new EventName { Code = EventCode.PatientCancelEvent } });
            rttPeriod.Add(new ClockTickingCompletedEvent { Name = new EventName { Code = EventCode.HospitalCancelEvent } });

            // Act
            var result = rttPeriod.IsSecondCancelByPatient(EventCode.PatientCancelEvent);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void IsSecondCancelByPatient_ReturnsFalse_WhenPatientCancelsTheSecondTimeAndHospitalOnce()
        {
            // Arrange
            var rttPeriod = new Non18WeekPeriod { Pathway = new Pathway { Patient = new Patient { Name = "John Doe", DateOfBirth = new DateTime(1990, 2, 2) } } };
            rttPeriod.Add(new ClockTickingCompletedEvent { Name = new EventName { Code = EventCode.PatientCancelEvent } });
            rttPeriod.Add(new ClockTickingCompletedEvent { Name = new EventName { Code = EventCode.HospitalCancelEvent } });

            // Act
            var result = rttPeriod.IsSecondCancelByPatient(EventCode.PatientCancelEvent);

            // Assert
            Assert.IsFalse(result);
        }


        [TestMethod]
        public void IsSecondDidNotAttend_ReturnsTrue_WhenPatientDoesNotAttendSecondTime()
        {
            // Arrange
            var rttPeriod = new Non18WeekPeriod { Pathway = new Pathway { Patient = new Patient { Name = "John Doe", DateOfBirth = new DateTime(1990, 2, 2) } } };
            rttPeriod.Add(new ClockTickingCompletedEvent { Name = new EventName { Code = EventCode.DidNotAttend } });

            // Act
            var result = rttPeriod.IsSecondDidNotAttend(EventCode.DidNotAttend);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void IsSecondDidNotAttend_ReturnsFalse_WhenPatientDoesNotAttendFirstTime()
        {
            // Arrange
            var rttPeriod = new Non18WeekPeriod { Pathway = new Pathway { Patient = new Patient { Name = "John Doe", DateOfBirth = new DateTime(1990, 2, 2) } } };

            // Act
            var result = rttPeriod.IsSecondDidNotAttend(EventCode.DidNotAttend);

            // Assert
            Assert.IsFalse(result);
        }

        private Period GetNon18WeekPeriod()
        {
            var period = new Non18WeekPeriod { Pathway = new Pathway { Patient = new Patient { Name = "John Doe", DateOfBirth = new DateTime(1990, 2, 2) }, PPINumber = "ppi"  }, Name = "period 1"  };
            period.Add(new ClockTickingCompletedEvent { EventDate = new DateTime(2000, 12, 25), TargetDate = new DateTime(2000, 12, 25) });
            period.Add(new ClockTickingCompletedEvent { EventDate = new DateTime(2000, 12, 27) });

            return period;
        }
    }
}
