using System.Collections.Generic;
using CPMS.Domain;

namespace CPMS.Report.Manager
{
    public interface IEventNameRepository
    {
        IEnumerable<EventName> Get();
    }
}
