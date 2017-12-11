using System.Collections.Generic;
using System.Linq;

namespace CPMS.Domain
{
    public class Pathway : ValidationBase
    {
        public Pathway()
        {
            Periods = new List<Period>();
        }

        private const int Length = 20;

        public string PPINumber { get; set; }

        public Patient Patient { get; set; }

        public string OrganizationCode { get; set; }

        public IList<Period> Periods { get; set; }

        public void Validate()
        {
            if (PPINumber.Length != Length)
            {
                OnValidationFailed(new RuleViolation
                {
                    Message = string.Format("Ppi number {0} should have exactly {1} characters.", PPINumber, Length)
                });
            }
        }

        public void Add(Period newPeriod)
        {
            if (newPeriod.IsActive && Periods.Any(p => p.IsActive))
            {
                OnValidationFailed(new RuleViolation
                {
                    Message = string.Format("Pathway {0} should have only an active period.", PPINumber),
                    Period = newPeriod
                });

            }
            foreach (var period in Periods.Where(period => period.StopDate == null))
            {
                OnValidationFailed(new RuleViolation
                {
                    Message = string.Format("{0} on pathway {1} should be closed.", period.Name, PPINumber),
                    Period = period
                });
            }
            
            Periods.Add(newPeriod);
        }
    }
}
