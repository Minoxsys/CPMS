using System;

namespace CPMS.Report.Manager
{
    public interface IClock
    {
        DateTime Today { get; }
    }
}
