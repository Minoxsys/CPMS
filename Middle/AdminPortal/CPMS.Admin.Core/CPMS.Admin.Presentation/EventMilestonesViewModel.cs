using System.Collections.Generic;

namespace CPMS.Admin.Presentation
{
    public class EventMilestonesViewModel
    {
        public IEnumerable<EventMilestoneViewModel> EventMilestones { get; set; }

        public int TotalNumberOfEventMilestones { get; set; }
    }
}
