using System.Collections.Generic;
using System.Linq;
using CPMS.Domain;
using CPMS.Report.Manager;

namespace CPMS.Report.Core.Adapters
{
    public class EventNameRepository : IEventNameRepository
    {
        private readonly UnitOfWork _unitOfWork;

        public EventNameRepository(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IEnumerable<EventName> Get()
        {
            return _unitOfWork.EventNames.ToArray();
        }
    }
}
