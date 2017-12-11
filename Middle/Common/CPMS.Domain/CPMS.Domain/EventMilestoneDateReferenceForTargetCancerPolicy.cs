using System.Linq;

namespace CPMS.Domain
{
    public class EventMilestoneDateReferenceForTargetCancerPolicy
    {
        public void ApplyTo(EventMilestone eventMilestone)
        {
            if (!eventMilestone.CompletedEvent.Cancer)
            {
                return;
            }

            var referralReceivedEvent =
                eventMilestone.CompletedEvent.Period.CompletedEvents.FirstOrDefault(completedEvent => completedEvent.Name.Code == EventCode.ReferralReceived);

            if (eventMilestone.Name.Code == EventCode.DiagnosticsTestResultAvailable ||
                eventMilestone.Name.Code == EventCode.AttendedOutpatientFirstAppointment)
            {
                if (referralReceivedEvent != null)
                {
                    eventMilestone.DateReferenceForTarget = referralReceivedEvent.EventDate;
                }
            }
        }
    }
}
