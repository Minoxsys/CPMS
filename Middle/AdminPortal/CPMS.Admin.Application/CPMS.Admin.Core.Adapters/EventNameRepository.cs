using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CPMS.Admin.Application;
using CPMS.Domain;

namespace CPMS.Admin.Core.Adapters
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
