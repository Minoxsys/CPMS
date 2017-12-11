using System.Linq;

namespace CPMS.Domain
{
    public class CompletedEventTargetDateCancerPolicy
    {
        public void ApplyTo(CompletedEvent completedEvent)
        {
            if (!completedEvent.Cancer)
            {
                return;
            }

            var referralReceivedEvent = completedEvent.Period.CompletedEvents.FirstOrDefault(@event => @event.Name.Code == EventCode.ReferralReceived);

            if (completedEvent.Name.Code == EventCode.DiagnosticsTestResultAvailable)
            {
                if (referralReceivedEvent != null)
                {
                    completedEvent.TargetDate =
                        referralReceivedEvent.EventDate.AddDays(
                            Constants.DignosticTestResultAvailableTargetNumberOfDaysForCancer);
                }
            }

            if (completedEvent.Name.Code == EventCode.AttendedOutpatientFirstAppointment)
            {
                if (referralReceivedEvent != null)
                {
                    completedEvent.TargetDate =
                        referralReceivedEvent.EventDate.AddDays(
                            Constants.OutpatientFirstAppointmentAttendedTargetNumberOfDaysForCancer);
                }
            }
        }
    }
}
