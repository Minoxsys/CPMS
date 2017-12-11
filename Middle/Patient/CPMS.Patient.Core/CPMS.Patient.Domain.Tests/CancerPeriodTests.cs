using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CPMS.Patient.Domain.Tests
{
    [TestClass]
    public class CancerPeriodTests
    {
        [TestMethod]
        public void GetDaysSpentAt_CorrectlyReturnsSum_WhenReferenceDateIsGreaterThanAllEventsDateInThePeriod()
        {
            // Arrange
            var cancerPeriod = GetCancerPeriod();

            // Act
            var result = cancerPeriod.GetDaysSpentAt(new DateTime(2001, 01, 02));

            // Assert
            Assert.AreEqual(5, result);
        }

        [TestMethod]
        public void GetDaysSpentAt_CorrectlyReturnsSum_WhenReferenceDateIsLessThanSomeEventsDateInThePeriod()
        {
            // Arrange
            var cancerPeriod = GetCancerPeriod();

            // Act
            var result = cancerPeriod.GetDaysSpentAt(new DateTime(2000, 12, 29));

            // Assert
            Assert.AreEqual(3, result);
        }

        [TestMethod]
        public void GetDaysRemainingAt_CorrectlyReturnsDifference_WhenPeriodIsCancerWithoutDiagnosticTestResultAvailable()
        {
            // Arrange
            var cancerPeriod = GetCancerPeriodWithoutDiagnosticTestResultAvailable();

            // Act
            var result = cancerPeriod.GetDaysRemainingAt(new DateTime(2000, 12, 15));

            // Assert
            Assert.AreEqual(47, result);
        }

        [TestMethod]
        public void GetDaysRemainingAt_CorrectlyReturnsDifference_WhenPeriodIsCancerWithDiagnosticTestResultAvailable()
        {
            // Arrange
            var cancerPeriod = GetCancerPeriodWithDiagnosticTestResultAvailable();

            // Act
            var result = cancerPeriod.GetDaysRemainingAt(new DateTime(2000, 12, 25));

            // Assert
            Assert.AreEqual(26, result);
        }

        [TestMethod]
        public void GetNumberOfPausedDays_CorrectlyReturnsPausedDays()
        {
            // Arrange
            var cancerPeriod = GetCancerPeriod();

            // Act
            var result = cancerPeriod.GetNumberOfPausedDays(new DateTime(2000, 12, 29), new DateTime(2001, 1, 1));

            // Assert
            Assert.AreEqual(2, result);
        }

        [TestMethod]
        public void GetDelayOrAdvancementDays_CorrectlyReturnsDelayOrAdvancementDays()
        {
            // Arrange
            var cancerPeriod = GetCancerPeriod();

            // Act
            var result = cancerPeriod.GetDelayOrAdvancementDays(new DateTime(2000, 12, 29));

            // Assert
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public void Add_RaisesValidationFailedEvent_WhenCurrentEventHasADateBeforeAnotherEvent()
        {
            // Arrange
            var cancerPeriod = GetCancerPeriodWithoutDiagnosticTestResultAvailable();
            var @event = new ClockTickingEvent { Code = EventCode.InpatientPatientAttendancePreOperativeAssessment, EventDate = new DateTime(2000, 12, 2), Period = cancerPeriod};
            Error eventRaised = null;

            cancerPeriod.ValidationFailed += delegate { eventRaised = new Error(); };

            // Act
            cancerPeriod.Add(@event);

            // Assert
            Assert.IsNotNull(eventRaised);
        }

        [TestMethod]
        public void Add_RaisesValidationFailedEvent_WhenCurrentEventDoesNotHaveTargetReferenceEvent()
        {
            //Arrange
            var @event = new ClockTickingEvent { Code = EventCode.AttendedOutpatientFirstAppointment, EventDate = new DateTime(2014, 5, 20),Period = new RTT18WeekPeriod{Pathway = new Pathway { PPINumber = "ppi" }, Name = "period 1"}};
            var cancerPeriod = new CancerPeriod();
            cancerPeriod.Add(new ClockStartingEvent { Code = EventCode.ReferralReceived, EventDate = new DateTime(2014, 5, 6), Period = new RTT18WeekPeriod { Pathway = new Pathway { PPINumber = "ppi" }, Name = "period 1" } });
            cancerPeriod.Add(new ClockTickingEvent { Code = EventCode.ReferralReview, EventDate = new DateTime(2014, 5, 10), Period = new RTT18WeekPeriod { Pathway = new Pathway { PPINumber = "ppi" }, Name = "period 1" } });
            cancerPeriod.Add(new ClockTickingEvent { Code = EventCode.BookedOutpatientFirstAppointment, EventDate = new DateTime(2014, 5, 12), Period = new RTT18WeekPeriod { Pathway = new Pathway { PPINumber = "ppi" }, Name = "period 1" } });
            cancerPeriod.Add(new ClockTickingEvent { Code = EventCode.OutpatientFirstAppointment, EventDate = new DateTime(2014, 5, 14), Period = new RTT18WeekPeriod { Pathway = new Pathway { PPINumber = "ppi" }, Name = "period 1" } });

            var plannedEvent = new PlannedEvent { Code = EventCode.OutcomedOutpatientFirstAppointment };

            var eventRaised = false;

            cancerPeriod.ValidationFailed += delegate { eventRaised = true; };

            //Act
            cancerPeriod.MapPlannedEventToEvent(@event, plannedEvent, EventCode.AttendedOutpatientFirstAppointment);

            //Assert
            Assert.IsTrue(eventRaised);
        }

        [TestMethod]
        public void Add_DoesNotRaiseValidationFailedEvent_WhenCurrentEventHasTargetReferenceEvent()
        {
            //Arrange
            var @event = new ClockTickingEvent { Code = EventCode.AttendedOutpatientFirstAppointment, EventDate = new DateTime(2014, 5, 20) };
            var cancerPeriod = new CancerPeriod();
            cancerPeriod.Add(new ClockStartingEvent { Code = EventCode.ReferralReceived, EventDate = new DateTime(2014, 5, 6) });
            cancerPeriod.Add(new ClockTickingEvent { Code = EventCode.ReferralReview, EventDate = new DateTime(2014, 5, 10) });
            cancerPeriod.Add(new ClockTickingEvent { Code = EventCode.BookedOutpatientFirstAppointment, EventDate = new DateTime(2014, 5, 12) });
            cancerPeriod.Add(new ClockTickingEvent { Code = EventCode.OutpatientFirstAppointment, EventDate = new DateTime(2014, 5, 14) });
            cancerPeriod.Add(@event);

            var plannedEvent = new PlannedEvent { Code = EventCode.OutcomedOutpatientFirstAppointment };

            var eventRaised = false;

            cancerPeriod.ValidationFailed += delegate { eventRaised = true; };

            //Act
            cancerPeriod.MapPlannedEventToEvent(@event, plannedEvent, EventCode.AttendedOutpatientFirstAppointment);

            //Assert
            Assert.IsFalse(eventRaised);
        }
        
        [TestMethod]
        public void Add_RaisesValidationFailedEvent_WhenPatientCancelsForTheSecondTimeAndPeriodIsNotStopped()
        {
            // Arrange
            var rttPeriod = GetCancerPeriod();
            var @event = new ClockTickingEvent { Code = EventCode.PatientCancelEvent, EventDate = new DateTime(2001, 1, 8), Period = rttPeriod};
            rttPeriod.Add(new ClockTickingEvent { Code = EventCode.PatientCancelEvent, EventDate = new DateTime(2001, 1, 9) });
            var eventRaised = false;

            rttPeriod.ValidationFailed += delegate { eventRaised = true; };

            // Act
            rttPeriod.Add(@event);

            // Assert
            Assert.IsNotNull(eventRaised);
        }

        [TestMethod]
        public void Add_RaisesValidationFailedEvent_WhenPatientIsAChildAndDoesNotAttendForTheSecondTimeAndPeriodIsNotStopped()
        {
            // Arrange
            var rttPeriod = GetCancerPeriod();
            var @event = new ClockTickingEvent { Code = EventCode.DidNotAttend, EventDate = new DateTime(2001, 1, 8), Period = rttPeriod};
            rttPeriod.Pathway.Patient.DateOfBirth = new DateTime(1990, 3, 5);
            rttPeriod.Add(new ClockTickingEvent { Code = EventCode.DidNotAttend, EventDate = new DateTime(2001, 1, 9) });

            var eventRaised = false;

            rttPeriod.ValidationFailed += delegate { eventRaised = true; };

            // Act
            rttPeriod.Add(@event);

            // Assert
            Assert.IsNotNull(eventRaised);
        }

        [TestMethod]
        public void Add_RaisesValidationFailedEvent_WhenPatientIsNotAChildAndDoesNotAttendForTheSecondTimeAndPeriodIsNotStopped()
        {
            // Arrange

            var rttPeriod = GetCancerPeriod();
            var @event = new ClockTickingEvent { Code = EventCode.DidNotAttend, EventDate = new DateTime(2001, 1, 8), Period = rttPeriod};

            rttPeriod.Pathway.Patient.DateOfBirth = new DateTime(1980, 3, 5);
            rttPeriod.Add(new ClockTickingEvent { Code = EventCode.DidNotAttend, EventDate = new DateTime(2001, 1, 9) });

            Error eventRaised = null;

            rttPeriod.ValidationFailed += delegate { eventRaised = new Error(); };

            // Act
            rttPeriod.Add(@event);

            // Assert
            Assert.IsNotNull(eventRaised);
        }
 
        [TestMethod]
        public void GetPauseValidationMessage_ReturnValidationMessageForCancer()
        {
            // Arrange
            var non18WeekPeriod = GetCancerPeriod();

            // Act
            var result = non18WeekPeriod.GetPauseValidationMessage;

            // Assert
            Assert.IsTrue(!string.IsNullOrEmpty(result));
        }

        [TestMethod]
        public void AbleToNotAttendFirstEvent_ReturnsTrue()
        {
            // Arrange
            var rttPeriod = GetCancerPeriod();

            // Act
            var result = rttPeriod.AbleToNotAttendFirstEvent;

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void ShouldCountForBreaches_ReturnsTrue()
        {
            // Arrange
            var cancerPeriod = GetCancerPeriod();

            // Act
            var result = cancerPeriod.ShouldCountForBreaches;

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void IsSecondCancelByPatient_ReturnsTrue_WhenPatientCancelsTheSecondTime()
        {
            // Arrange
            var rttPeriod = new CancerPeriod();
            rttPeriod.Add(new ClockTickingEvent { Code = EventCode.PatientCancelEvent });

            // Act
            var result = rttPeriod.IsSecondCancelByPatient(EventCode.PatientCancelEvent);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void IsSecondCancelByPatient_ReturnsTrue_WhenPatientCancelsThreeTimesAndHospitalOnce()
        {
            // Arrange
            var rttPeriod = new CancerPeriod();
            rttPeriod.Add(new ClockTickingEvent { Code = EventCode.PatientCancelEvent });
            rttPeriod.Add(new ClockTickingEvent { Code = EventCode.PatientCancelEvent });
            rttPeriod.Add(new ClockTickingEvent { Code = EventCode.HospitalCancelEvent });

            // Act
            var result = rttPeriod.IsSecondCancelByPatient(EventCode.PatientCancelEvent);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void IsSecondCancelByPatient_ReturnsFalse_WhenPatientCancelsTheSecondTimeAndHospitalOnce()
        {
            // Arrange
            var rttPeriod = new CancerPeriod();
            rttPeriod.Add(new ClockTickingEvent { Code = EventCode.PatientCancelEvent });
            rttPeriod.Add(new ClockTickingEvent { Code = EventCode.HospitalCancelEvent });

            // Act
            var result = rttPeriod.IsSecondCancelByPatient(EventCode.PatientCancelEvent);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void IsSecondDidNotAttend_ReturnsTrue_WhenPatientDoesNotAttendSecondTime()
        {
            // Arrange
            var rttPeriod = new CancerPeriod();
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
            var rttPeriod = new CancerPeriod();

            // Act
            var result = rttPeriod.IsSecondDidNotAttend(EventCode.DidNotAttend);

            // Assert
            Assert.IsFalse(result);
        }

        private Period GetCancerPeriodWithoutDiagnosticTestResultAvailable()
        {
            var period = new CancerPeriod { StartDate = new DateTime(2000, 12, 01), Pathway = new Pathway()};
            period.Add(new ClockStartingEvent { EventDate = new DateTime(2000, 12, 01), TargetDate = new DateTime(2000, 12, 01), Code = EventCode.ReferralReceived });
            period.Add(new ClockTickingEvent { EventDate = new DateTime(2000, 12, 10), Code = EventCode.DiagnosticsOrderPlaced });

            return period;
        }

        private Period GetCancerPeriodWithDiagnosticTestResultAvailable()
        {
            var period = new CancerPeriod { StartDate = new DateTime(2000, 12, 01) };
            period.Add(new ClockStartingEvent { EventDate = new DateTime(2000, 12, 01), TargetDate = new DateTime(2000, 12, 01), Code = EventCode.ReferralReceived });
            period.Add(new ClockTickingEvent { EventDate = new DateTime(2000, 12, 10), Code = EventCode.DiagnosticsOrderPlaced });
            period.Add(new ClockTickingEvent { EventDate = new DateTime(2000, 12, 20), Code = EventCode.DiagnosticsTestResultAvailable });

            return period;
        }

        private Period GetCancerPeriod()
        {
            var period = new CancerPeriod {Pathway = new Pathway {Patient = new Patient{Name = "John Doe", DateOfBirth = new DateTime(1990, 2, 2)}}};
            period.Add(new ClockStartingEvent { EventDate = new DateTime(2000, 12, 25), TargetDate = new DateTime(2000, 12, 25) });
            period.Add(new ClockPausingEvent { EventDate = new DateTime(2000, 12, 27) });
            period.Add(new ClockStartingEvent { EventDate = new DateTime(2000, 12, 29), TargetDate = new DateTime(2000, 12, 30) });
            period.Add(new ClockPausingEvent { EventDate = new DateTime(2000, 12, 30), TargetDate = new DateTime(2001, 1, 3) });
            period.Add(new ClockStartingEvent { EventDate = new DateTime(2001, 1, 1), TargetDate = new DateTime(2001, 1, 3) });
            period.Add(new ClockStoppingEvent { EventDate = new DateTime(2001, 1, 2), TargetDate = new DateTime(2001, 1, 6) });

            return period;
        }
    }
}
