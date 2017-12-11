namespace CPMS.Patient.Domain
{
    public class PlannedEventTargetNumberOfDaysCancerPolicy
    {
        public void ApplyTo(PlannedEvent plannedEvent)
        {
            if (!plannedEvent.Event.Cancer)
            {
                return;
            }

            //if (plannedEvent.Code == EventCode.DiagnosticsTestResultAvailable)
            //{
            //    plannedEvent.TargetNumberOfDays = Constants.DignosticTestResultAvailableTargetNumberOfDaysForCancer;
            //}

            //if (plannedEvent.Code == EventCode.AttendedOutpatientFirstAppointment)
            //{
            //    plannedEvent.TargetNumberOfDays =
            //        Constants.OutpatientFirstAppointmentAttendedTargetNumberOfDaysForCancer;
            //}
        }
    }
}
