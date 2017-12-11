using System.ComponentModel;

namespace CPMS.Admin.Presentation
{
    public enum EventCode
    {
        [Description("Referral Received")]
        ReferralReceived,
        [Description("Referral Review")]
        ReferralReview,
        [Description("Booked Outpatient First Appointment")]
        BookedOutpatientFirstAppointment,
        [Description("Outpatient First Appointment")]
        OutpatientFirstAppointment,
        [Description("Attended Outpatient First Appointment")]
        AttendedOutpatientFirstAppointment,
        [Description("Outcomed Outpatient First Appointment")]
        OutcomedOutpatientFirstAppointment,
        [Description("Diagnostics Order Placed")]
        DiagnosticsOrderPlaced,
        [Description("Diagnostics Test Result Available")]
        DiagnosticsTestResultAvailable,
        [Description("Letter Sent")]
        LetterSent,
        [Description("Outpatient Discharged")]
        OutpatientDischarged,
        [Description("Booked Outpatient Follow Up Appointment")]
        BookedOutpatientFollowUpAppointment,
        [Description("Outpatient Follow Up Appointment")]
        OutpatientFollowUpAppointment,
        [Description("Attended Outpatient Follow Up Appointment")]
        AttendedOutpatientFollowUpAppointment,
        [Description("Outcomed Outpatient Follow Up Appointment")]
        OutcomedOutpatientFollowUpAppointment,
        [Description("Patient Added To Inpatient Waiting List")]
        PatientAddedToInpatientWaitingList,
        [Description("Inpatient TCI Offer")]
        InpatientTCIOffer,
        [Description("Inpatient TCI Agreed")]
        InpatientTCIAgreed,
        [Description("Inpatient TCI")]
        InpatientTCI,
        [Description("Booked Inpatient Pre Operative Assessment")]
        BookedInpatientPreOperativeAssessment,
        [Description("Inpatient Pre Operative Assessment")]
        InpatientPreOperativeAssessment,
        [Description("Inpatient Patient Attendance Pre Operative Assessment")]
        InpatientPatientAttendancePreOperativeAssessment,
        [Description("Inpatient Admitted")]
        InpatientAdmitted,
        [Description("Expected Inpatient Discharged")]
        ExpectedInpatientDischarged,
        [Description("Inpatient Discharged")]
        InpatientDischarged,
        [Description("Episode Clinically Coded")]
        EpisodeClinicallyCoded,
        [Description("Booked Post Inpatient First Outpatient Appointment")]
        BookedPostInpatientFirstOutpatientAppointment,
        [Description("Post Inpatient First Outpatient Follow Up Appointment")]
        PostInpatientFirstOutpatientFollowUpAppointment,
        [Description("Attended Post Inpatient First Outpatient Follow Up Appointment")]
        AttendedPostInpatientFirstOutpatientFollowUpAppointment,
        [Description("Outcomed Post Inpatient First Outpatient Follow Up Appointment")]
        OutcomedPostInpatientFirstOutpatientFollowUpAppointment,
        [Description("Booked Post Inpatient Second Outpatient Appointment")]
        BookedPostInpatientSecondOutpatientAppointment,
        [Description("Post Inpatient Second Outpatient Follow Up Appointment")]
        PostInpatientSecondOutpatientFollowUpAppointment,
        [Description("Attended Post Inpatient Second Outpatient Follow Up Appointment")]
        AttendedPostInpatientSecondOutpatientFollowUpAppointment,
        [Description("Outcomed Post Inpatient Second Outpatient Follow Up Appointment")]
        OutcomedPostInpatientSecondOutpatientFollowUpAppointment,
        [Description("Patient Cancel")]
        PatientCancelEvent,
        [Description("Hospital Cancel")]
        HospitalCancelEvent,
        [Description("Pathway Completion")]
        PathwayCompletion,
        [Description("Did Not Attend")]
        DidNotAttend
    }
}