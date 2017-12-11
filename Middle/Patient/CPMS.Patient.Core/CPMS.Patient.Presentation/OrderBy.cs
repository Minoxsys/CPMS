using System.ComponentModel;

namespace CPMS.Patient.Presentation
{
    public enum OrderBy
    {
        [Description("PatientName")]
        PatientName,

        [Description("EventDescription")]
        EventDescription,

        [Description("Specialty")]
        Specialty,

        [Description("Clinician")]
        Clinician,

        [Description("PostBreachDays")]
        PostBreachDays,

        [Description("Age")]
        Age,

        [Description("Hospital")]
        Hospital,

        [Description("DateOfBirth")]
        DateOfBirth,

        [Description("TargetDate")]
        TargetDate,

        [Description("ActualDate")]
        ActualDate,

        [Description("ImportDate")]
        ImportDate,

        [Description("DaysRemainingInPeriod")]
        DaysRemainingInPeriod,

        [Description("DaysInPeriod")]
        DaysInPeriod
    }
}
