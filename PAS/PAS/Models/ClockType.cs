using System.ComponentModel;

namespace PAS.Models
{
    public enum ClockType
    {
        [Description("Clock Starting")]
        ClockStarting,
        [Description("Clock Ticking")]
        ClockTicking,
        [Description("Clock Pausing")]
        ClockPausing,
        [Description("Clock Stopping")]
        ClockStopping
    }
}
