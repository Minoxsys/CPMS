using System.Linq;

namespace CPMS.Patient.Domain
{
    public class EventTargetDateCancerPolicy
    {
        public void ApplyTo(Event evt)
        {
            if (!evt.Cancer)
            {
                return;
            }

            var referralReceivedEvent = evt.Period.Events.FirstOrDefault(ev => ev.Code == EventCode.ReferralReceived);

            if (evt.Code == EventCode.DiagnosticsTestResultAvailable)
            {
                if (referralReceivedEvent != null)
                {
                    evt.TargetDate =
                        referralReceivedEvent.EventDate.AddDays(
                            Constants.DignosticTestResultAvailableTargetNumberOfDaysForCancer);
                }
            }

            if (evt.Code == EventCode.AttendedOutpatientFirstAppointment)
            {
                if (referralReceivedEvent != null)
                {
                    evt.TargetDate =
                        referralReceivedEvent.EventDate.AddDays(
                            Constants.OutpatientFirstAppointmentAttendedTargetNumberOfDaysForCancer);
                }
            }
        }
    }
}
