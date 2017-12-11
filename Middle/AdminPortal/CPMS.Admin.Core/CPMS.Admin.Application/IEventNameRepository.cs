using System.Collections.Generic;
using CPMS.Domain;

namespace CPMS.Admin.Application
{
    public interface IEventNameRepository
    {
        IEnumerable<EventName> Get();
    }
}
