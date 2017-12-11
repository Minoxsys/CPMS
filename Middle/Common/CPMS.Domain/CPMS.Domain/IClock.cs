using System;

namespace CPMS.Domain
{
    public interface IClock
    {
        DateTime TodayDate { get; }

        DateTime TodayDateAndTime { get; }
    }
}
