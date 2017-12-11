using CPMS.Patient.Domain;

namespace CPMS.Patient.Manager
{
    public class LiteEventBreachInfo
    {
        public EventCode EventCode { get; set; }

        public EventBreachStatus Status { get; set; }

        public int DaysForStatus { get; set; }
    }
}
