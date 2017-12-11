using System.Collections.Generic;

namespace CPMS.Configuration
{
    public class SourceEvent
    {
        public ConfigurationEventCode SourceCode { get; set; }
        
        public IList<DestinationEvent> NextPossibleEvents { get; set; }
    }
}
