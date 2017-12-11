using System.Collections.Generic;
using CPMS.Domain;

namespace CPMS.Configuration
{
    public class SourceEvent
    {
        public int Id { get; set; }

        public EventName SourceName { get; set; }

        public IList<DestinationEvent> NextPossibleEvents { get; set; }
    }
}
