using System.Linq;

namespace CPMS.Patient.Domain
{
    public class PlannedEventDateReferenceForTargetCancerPolicy
    {
        public void ApplyTo(PlannedEvent plannedEvent)
        {
            if (!plannedEvent.Event.Cancer)
            {
                return;
            }

            var referralReceivedEvent =
                plannedEvent.Event.Period.Events.FirstOrDefault(ev => ev.Code == EventCode.ReferralReceived);

            if (plannedEvent.Code == EventCode.DiagnosticsTestResultAvailable ||
                plannedEvent.Code == EventCode.AttendedOutpatientFirstAppointment)
            {
                if (referralReceivedEvent != null)
                {
                    plannedEvent.DateReferenceForTarget = referralReceivedEvent.EventDate;
                }
            }
        }
    }
}
