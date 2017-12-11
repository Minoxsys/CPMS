using System;
using System.Configuration;

namespace CPMS.Breach.Engine.Adapters
{
    public class Clock : IClock
    {
        public DateTime UtcNow
        {
            get
            {
                DateTime configUtcNow;

                return DateTime.TryParse(ConfigurationManager.AppSettings["UtcNow"], out configUtcNow) ? configUtcNow : DateTime.Now;
            }
        }
    }
}
