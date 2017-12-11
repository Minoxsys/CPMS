using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using CPMS.Configuration;
using IDestinationRepository = CPMS.Admin.Application.IDestinationRepository;

namespace CPMS.Admin.Core.Adapters
{
    public class DestinationRepository:IDestinationRepository
    {
        private readonly UnitOfWork _unitOfWork;

        public DestinationRepository(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IEnumerable<DestinationEvent> Get(Expression<Func<DestinationEvent, bool>> criteria)
        {
            return _unitOfWork.DestinationEvents.Where(criteria).ToArray();
        }
    }
}
