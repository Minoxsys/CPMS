using System.Collections.Generic;
using CPMS.Configuration;

namespace CPMS.Admin.Application
{
    public class PlannedEventFilterInputInfo
    {
        public IList<ConfigurationEventCode> ParentEventCodes { get; set; }

        public IList<ConfigurationEventCode> PlannedEventCodes { get; set; }

        public IList<ConfigurationEventCode> EventForTargetCodes { get; set; }

        public string ParentEventValue { get; set; }

        public string PlannedEventValue { get; set; }

        public string EventForTargetValue { get; set; }
    }
}
