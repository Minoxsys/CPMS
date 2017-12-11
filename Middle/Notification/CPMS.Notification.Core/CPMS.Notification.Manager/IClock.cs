using System;

namespace CPMS.Notification.Manager
{
    public interface IClock
    {
        DateTime TodayDate { get; }

        DateTime TodayDateAndTime { get; }
    }
}
