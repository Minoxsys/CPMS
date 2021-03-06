﻿namespace CPMS.Domain
{
    public enum EventCode
    {
        ReferralReceived,
        ReferralReview,
        BookedOutpatientFirstAppointment,
        OutpatientFirstAppointment,
        AttendedOutpatientFirstAppointment,
        OutcomedOutpatientFirstAppointment,
        DiagnosticsOrderPlaced,
        DiagnosticsTestResultAvailable,
        LetterSent,
        OutpatientDischarged,
        BookedOutpatientFollowUpAppointment,
        OutpatientFollowUpAppointment,
        AttendedOutpatientFollowUpAppointment,
        OutcomedOutpatientFollowUpAppointment,
        PatientAddedToInpatientWaitingList,
        InpatientTCIOffer,
        InpatientTCIAgreed,
        InpatientTCI,
        BookedInpatientPreOperativeAssessment,
        InpatientPreOperativeAssessment,
        InpatientPatientAttendancePreOperativeAssessment,
        InpatientAdmitted,
        ExpectedInpatientDischarged,
        InpatientDischarged,
        EpisodeClinicallyCoded,
        BookedPostInpatientFirstOutpatientAppointment,
        PostInpatientFirstOutpatientFollowUpAppointment,
        AttendedPostInpatientFirstOutpatientFollowUpAppointment,
        OutcomedPostInpatientFirstOutpatientFollowUpAppointment,
        BookedPostInpatientSecondOutpatientAppointment,
        PostInpatientSecondOutpatientFollowUpAppointment,
        AttendedPostInpatientSecondOutpatientFollowUpAppointment,
        OutcomedPostInpatientSecondOutpatientFollowUpAppointment,
        PatientCancelEvent,
        HospitalCancelEvent,
        PathwayCompletion,
        DidNotAttend
    }
}