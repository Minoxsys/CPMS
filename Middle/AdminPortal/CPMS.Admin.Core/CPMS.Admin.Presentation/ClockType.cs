using System.ComponentModel;

namespace CPMS.Admin.Presentation
{
    public enum ClockType
    {
        [Description("Will Start")]
        ClockStarting,
        [Description("May Pause")]
        ClockPausing,
        [Description("Will Stop")]
        ClockStopping,
        [Description("None")]
        None
    }
}
