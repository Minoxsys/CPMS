using System.ComponentModel.DataAnnotations;

namespace CPMS.Admin.Presentation
{
    public class EventMilestoneViewModel
    {
        public int EventMilestoneId { get; set; }

        public string EventMilestoneDescription { get; set; }

        [RegularExpression("[0-9]*$", ErrorMessage = "Please enter a number ")]
        public int? TargetNumberOfDays { get; set; }

        public string IsMandatory { get; set; }

        public string ParentEventDescription { get; set; }

        public string EventForDateReferenceForTarget { get; set; }

        public string ClockType { get; set; }
    }
}
