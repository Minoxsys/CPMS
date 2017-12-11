using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CPMS.Patient.Domain.Tests
{
    [TestClass]
    public class RTT18WeekPeriodTests
    {
        [TestMethod]
        public void GetDaysSpentAt_CorrectlyReturnsSum_WhenReferenceDateIsGreaterThanAllEventsDateInThePeriod()
        {
            // Arrange
            var rttPeriod = GetRTT18WeekPeriod();

            // Act
            var result = rttPeriod.GetDaysSpentAt(new DateTime(2001, 01, 02));

            // Assert
            Assert.AreEqual(5, result);
        }

        [TestMethod]
        public void GetDaysSpentAt_CorrectlyReturnsSum_WhenReferenceDateIsLessThanSomeEventsDateInThePeriod()
        {
            // Arrange
            var rttPeriod = GetRTT18WeekPeriod();

            // Act
            var result = rttPeriod.GetDaysSpentAt(new DateTime(2000, 12, 29));

            // Assert
            Assert.AreEqual(3, result);
        }

        [TestMethod]
        public void GetDaysRemainingAt_CorrectlyReturnsDifference_WhenReferenceDateIsGreaterThanAllEventsDateInThePeriod()
        {
            // Arrange
            var rttPeriod = GetRTT18WeekPeriod();

            // Act
            var result = rttPeriod.GetDaysRemainingAt(new DateTime(2001, 01, 02));

            // Assert
            Assert.AreEqual(121, result);
        }

        [TestMethod]
        public void GetDaysRemainingAt_CorrectlyReturnsDifference_WhenReferenceDateIsLessThanSomeEventsDateInThePeriod()
        {
            // Arrange
            var rttPeriod = GetRTT18WeekPeriod();

            // Act
            var result = rttPeriod.GetDaysRemainingAt(new DateTime(2000, 12, 29));

            // Assert
            Assert.AreEqual(123, result);
        }

        [TestMethod]
        public void GetNumberOfPausedDays_CorrectlyReturnsPausedDays()
        {
            // Arrange
            var rttPeriod = GetRTT18WeekPeriod();

            // Act
            var result = rttPeriod.GetNumberOfPausedDays(new DateTime(2000, 12, 29), new DateTime(2001, 1, 1));

            // Assert
            Assert.AreEqual(2, result);
        }

        [TestMethod]
        public void GetDelayOrAdvancementDays_CorrectlyReturnsDelayOrAdvancementDays()
        {
            // Arrange
            var rttPeriod = GetRTT18WeekPeriod();

            // Act
            var result = rttPeriod.GetDelayOrAdvancementDays(new DateTime(2000, 12, 29));

            // Assert
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public void Add_RaisesValidationFailedEvent_WhenCurrentEventHasADateBeforeAnotherEvent()
        {
            // Arrange
            var rttPeriod = GetRTT18WeekPeriod();
            var @event = new ClockTickingEvent { Code = EventCode.InpatientPatientAttendancePreOperativeAssessment, EventDate = new DateTime(2000, 12, 26), Period = rttPeriod};
            Error eventRaised = null;

            rttPeriod.ValidationFailed += delegate { eventRaised = new Error(); };

            // Act
            rttPeriod.Add(@event);

            // Assert
            Assert.IsNotNull(eventRaised);
        }

        [TestMethod]
        public void MapPlannedEventToEvent_RaisesValidationFailedEvent_WhenCurrentEventDoesNotHaveTargetReferenceEvent()
        {
            //Arrange

            var rttPeriod = new RTT18WeekPeriod{Pathway = new Pathway()};
            var @event = new ClockTickingEvent { Code = EventCode.AttendedOutpatientFirstAppointment, EventDate = new DateTime(2014, 5, 20), Period = rttPeriod};
            rttPeriod.Add(new ClockStartingEvent { Code = EventCode.ReferralReceived, EventDate = new DateTime(2014, 5, 6) });
            rttPeriod.Add(new ClockTickingEvent { Code = EventCode.ReferralReview, EventDate = new DateTime(2014, 5, 10) });
            rttPeriod.Add(new ClockTickingEvent { Code = EventCode.BookedOutpatientFirstAppointment, EventDate = new DateTime(2014, 5, 12) });
            rttPeriod.Add(new ClockTickingEvent { Code = EventCode.OutpatientFirstAppointment, EventDate = new DateTime(2014, 5, 14) });

            var plannedEvent = new PlannedEvent { Code = EventCode.OutcomedOutpatientFirstAppointment };

            Error eventRaised = null;

            rttPeriod.ValidationFailed += delegate { eventRaised = new Error(); };

            //Act
            rttPeriod.MapPlannedEventToEvent(@event, plannedEvent, EventCode.AttendedOutpatientFirstAppointment);

            //Assert
            Assert.IsNotNull(eventRaised);
        }

        [TestMethod]
        public void MapPlannedEventToEvent_DoesNotRaiseValidationFailedEvent_WhenCurrentEventHasTargetReferenceEvent()
        {
            //Arrange
            var @event = new ClockTickingEvent { Code = EventCode.AttendedOutpatientFirstAppointment, EventDate = new DateTime(2014, 5, 20) };
            var rttPeriod = new RTT18WeekPeriod();
            rttPeriod.Add(new ClockStartingEvent { Code = EventCode.ReferralReceived, EventDate = new DateTime(2014, 5, 6) });
            rttPeriod.Add(new ClockTickingEvent { Code = EventCode.ReferralReview, EventDate = new DateTime(2014, 5, 10) });
            rttPeriod.Add(new ClockTickingEvent { Code = EventCode.BookedOutpatientFirstAppointment, EventDate = new DateTime(2014, 5, 12) });
            rttPeriod.Add(new ClockTickingEvent { Code = EventCode.OutpatientFirstAppointment, EventDate = new DateTime(2014, 5, 14) });
            rttPeriod.Add(@event);

            var plannedEvent = new PlannedEvent { Code = EventCode.OutcomedOutpatientFirstAppointment };

            var eventRaised = false;

            rttPeriod.ValidationFailed += delegate { eventRaised = true; };

            //Act
            rttPeriod.MapPlannedEventToEvent(@event, plannedEvent, EventCode.AttendedOutpatientFirstAppointment);

            //Assert
            Assert.IsFalse(eventRaised);
        }

        [TestMethod]
        public void GetPauseValidationMessage_ReturnStringEmpty()
        {
            // Arrange
            var non18WeekPeriod = GetRTT18WeekPeriod();

            // Act
            var result = non18WeekPeriod.GetPauseValidationMessage;

            // Assert
            Assert.IsTrue(string.IsNullOrEmpty(result));
        }

        [TestMethod]
        public void AbleToNotAttendFirstEvent_ReturnsFalse()
        {
            // Arrange
            var rttPeriod = GetRTT18WeekPeriod();

            // Act
            var result = rttPeriod.AbleToNotAttendFirstEvent;

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void ShouldCountForBreaches_ReturnsTrue()
        {
            // Arrange
            var rttPeriod = GetRTT18WeekPeriod();

            // Act
            var result = rttPeriod.ShouldCountForBreaches;

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void IsSecondCancelByPatient_ReturnsTrue_WhenPatientCancelsTheSecondTime()
        {
            // Arrange
            var rttPeriod = new RTT18WeekPeriod { Pathway = new Pathway { Patient = new Patient { Name = "John Doe", DateOfBirth = new DateTime(1990, 2, 2) } } };
            rttPeriod.Add(new ClockTickingEvent {Code = EventCode.PatientCancelEvent});

            // Act
            var result = rttPeriod.IsSecondCancelByPatient(EventCode.PatientCancelEvent);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void IsSecondCancelByPatient_ReturnsTrue_WhenPatientCancelsThreeTimesAndHospitalOnce()
        {
            // Arrange
            var rttPeriod = new RTT18WeekPeriod { Pathway = new Pathway { Patient = new Patient { Name = "John Doe", DateOfBirth = new DateTime(1990, 2, 2) } } };
            rttPeriod.Add(new ClockTickingEvent {Code = EventCode.PatientCancelEvent});
            rttPeriod.Add(new ClockTickingEvent {Code = EventCode.PatientCancelEvent});
            rttPeriod.Add(new ClockTickingEvent {Code = EventCode.HospitalCancelEvent});

            // Act
            var result = rttPeriod.IsSecondCancelByPatient(EventCode.PatientCancelEvent);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void IsSecondCancelByPatient_ReturnsFalse_WhenPatientCancelsTheSecondTimeAndHospitalOnce()
        {
            // Arrange
            var rttPeriod = new RTT18WeekPeriod { Pathway = new Pathway { Patient = new Patient { Name = "John Doe", DateOfBirth = new DateTime(1990, 2, 2) } } };
            rttPeriod.Add(new ClockTickingEvent {Code = EventCode.PatientCancelEvent});
            rttPeriod.Add(new ClockTickingEvent {Code = EventCode.HospitalCancelEvent});

            // Act
            var result = rttPeriod.IsSecondCancelByPatient(EventCode.PatientCancelEvent);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void IsSecondDidNotAttend_ReturnsTrue_WhenPatientDoesNotAttendSecondTime()
        {
            // Arrange
            var rttPeriod = new RTT18WeekPeriod { Pathway = new Pathway { Patient = new Patient { Name = "John Doe", DateOfBirth = new DateTime(1990, 2, 2) } } };
            rttPeriod.Add(new ClockTickingEvent { Code = EventCode.DidNotAttend });

            // Act
            var result = rttPeriod.IsSecondDidNotAttend(EventCode.DidNotAttend);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void IsSecondDidNotAttend_ReturnsFalse_WhenPatientDoesNotAttendFirstTime()
        {
            // Arrange
            var rttPeriod = new RTT18WeekPeriod { Pathway = new Pathway { Patient = new Patient { Name = "John Doe", DateOfBirth = new DateTime(1990, 2, 2) } } };

            // Act
            var result = rttPeriod.IsSecondDidNotAttend(EventCode.DidNotAttend);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void GetPeriodStatus_ReturnsPausedStatus_WhenLastEventHasPausedStatus()
        {
            // Arrange
            var period = new RTT18WeekPeriod();
            period.Add(new ClockStartingEvent { EventDate = new DateTime(2000, 12, 25), TargetDate = new DateTime(2000, 12, 25) });
            period.Add(new ClockPausingEvent { EventDate = new DateTime(2000, 12, 27) });

            // Act
            var result = period.GetPeriodStatus();

            // Assert
            Assert.AreEqual(result, PeriodStatus.Paused);
        }

        [TestMethod]
        public void GetPeriodStatus_ReturnsEndedStatus_WhenPeriodHasStopDate()
        {
            // Arrange
            var period = new RTT18WeekPeriod { StopDate = new DateTime(2000, 12, 27) }; 
            period.Add(new ClockStartingEvent { EventDate = new DateTime(2000, 12, 25), TargetDate = new DateTime(2000, 12, 25) });
            period.Add(new ClockStoppingEvent { EventDate = new DateTime(2000, 12, 27) });

            // Act
            var result = period.GetPeriodStatus();

            // Assert
            Assert.AreEqual(result, PeriodStatus.Ended);
        }

        [TestMethod]
        public void GetPeriodStatus_ReturnsInProgressStatus_WhenIsNoStopDateAndTheLastHasNotPausedStatus()
        {
            // Arrange
            var period = new RTT18WeekPeriod();
            period.Add(new ClockStartingEvent { EventDate = new DateTime(2000, 12, 25), TargetDate = new DateTime(2000, 12, 25) });

            // Act
            var result = period.GetPeriodStatus();

            // Assert
            Assert.AreEqual(result, PeriodStatus.InProgress);
        }

        private Period GetRTT18WeekPeriod()
        {
            var period = new RTT18WeekPeriod {Pathway = new Pathway {Patient = new Patient {Name = "John Doe", DateOfBirth = new DateTime(1990, 2, 2)}}};
            period.Add(new ClockStartingEvent {EventDate = new DateTime(2000, 12, 25), TargetDate = new DateTime(2000, 12, 25)});
            period.Add(new ClockPausingEvent {EventDate = new DateTime(2000, 12, 27)});
            period.Add(new ClockStartingEvent {EventDate = new DateTime(2000, 12, 29), TargetDate = new DateTime(2000, 12, 30)});
            period.Add(new ClockPausingEvent {EventDate = new DateTime(2000, 12, 30), TargetDate = new DateTime(2001, 1, 3)});
            period.Add(new ClockStartingEvent {EventDate = new DateTime(2001, 1, 1), TargetDate = new DateTime(2001, 1, 3)});
            period.Add(new ClockStoppingEvent {EventDate = new DateTime(2001, 1, 2), TargetDate = new DateTime(2001, 1, 6)});

            return period;
        }
    }
}