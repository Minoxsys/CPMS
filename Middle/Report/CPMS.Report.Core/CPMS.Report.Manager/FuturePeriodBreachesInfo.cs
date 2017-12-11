
namespace CPMS.Report.Manager
{
    public class FuturePeriodBreachesInfo
    {
        public string Hospital { get; set; }

        public string Specialty { get; set; }

        public string Clinician { get; set; }

        public int NumberOfBreaches { get; set; }

        public int WeeksToBreach { get; set; }
    }
}
