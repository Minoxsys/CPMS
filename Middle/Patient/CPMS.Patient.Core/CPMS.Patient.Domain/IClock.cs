using System;

namespace CPMS.Patient.Domain
{
    public interface IClock
    {
        DateTime TodayDate { get; }

        DateTime TodayDateAndTime { get; }
    }
}
