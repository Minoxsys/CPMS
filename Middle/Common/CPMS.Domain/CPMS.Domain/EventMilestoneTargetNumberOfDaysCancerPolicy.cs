namespace CPMS.Domain
{
    public class EventMilestoneTargetNumberOfDaysCancerPolicy
    {
        public void ApplyTo(EventMilestone eventMilestone)
        {
            if (!eventMilestone.CompletedEvent.Cancer)
            {
                return;
            }

            if (eventMilestone.Name.Code == EventCode.DiagnosticsTestResultAvailable)
            {
                eventMilestone.TargetNumberOfDays = Constants.DignosticTestResultAvailableTargetNumberOfDaysForCancer;
            }

            if (eventMilestone.Name.Code == EventCode.AttendedOutpatientFirstAppointment)
            {
                eventMilestone.TargetNumberOfDays =
                    Constants.OutpatientFirstAppointmentAttendedTargetNumberOfDaysForCancer;
            }
        }
    }
}
