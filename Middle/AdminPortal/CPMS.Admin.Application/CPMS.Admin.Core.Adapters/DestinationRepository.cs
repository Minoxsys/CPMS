using System.Linq;
using CPMS.Configuration;

namespace CPMS.Admin.Core.Adapters
{
    public class DestinationRepository:IDestinationRepository
    {
        public void Update(DestinationEvent destinationEvent)
        {
            using (var unitOfWork = new UnitOfWork())
            {
                var existingEvent = unitOfWork.DestinationEvents.Single(d => d.Id == destinationEvent.Id);
                existingEvent.TargetNumberOfDays = destinationEvent.TargetNumberOfDays;
                unitOfWork.SaveChanges();
            }
        }
    }
}
