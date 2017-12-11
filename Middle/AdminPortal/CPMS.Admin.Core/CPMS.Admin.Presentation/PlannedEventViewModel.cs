using System.ComponentModel.DataAnnotations;

namespace CPMS.Admin.Presentation
{
    public class PlannedEventViewModel
    {
        public int PlannedEventId { get; set; }

        public string PlannedEventCode { get; set; }

        [RegularExpression("[0-9]*$", ErrorMessage = "Please enter a number ")]
        public int? TargetNumberOfDays { get; set; }

        public string IsMandatory { get; set; }

        public string ParentEventCode { get; set; }

        public string EventForDateReferenceForTarget { get; set; }

        public string ClockType { get; set; }
    }
}
