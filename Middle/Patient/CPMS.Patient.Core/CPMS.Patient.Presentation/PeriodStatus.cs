using System.ComponentModel;

namespace CPMS.Patient.Presentation
{
    public enum PeriodStatus
    {
        [Description("Ended")]
        Ended,

        [Description("In progress")]
        InProgress,

        [Description("Paused")]
        Paused
    }
}
