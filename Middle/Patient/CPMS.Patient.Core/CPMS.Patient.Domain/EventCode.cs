using System.ComponentModel;

namespace CPMS.Patient.Domain
{
    public enum EventCode
    {
        [Description("Referral Received")]
        ReferralReceived,
        [Description("Further Information Required")]
        FurtherInformationRequired,
        [Description("Add to triage waiting list")]
        AddToTriageWaitingList,
        [Description("Triage Referral")]
        TriageReferral,
        [Description("Add patient to Appointment Waiting List")]
        AddPatientToAppointmentWaitingList,
        [Description("Newly diagnosed Structured education")]
        NewlyDiagnosedStructuredEducation,
        [Description("Book Appt")]
        BookAppt,
        [Description("Appointment Date")]
        AppointmentDate,
        [Description("Patient Attends")]
        PatientAttends,
        [Description("Outcome Appointment")]
        OutcomeAppointment
    }
}
