using System;
using System.Configuration;

namespace CPMS.Report.Core.Adapters
{
    public class Clock : Manager.IClock
    {
       public DateTime Today
        {
            get
            {
                DateTime todayDate;

                return DateTime.TryParse(ConfigurationManager.AppSettings["UtcNow"], out todayDate) ? todayDate.Date : DateTime.UtcNow.Date;
            }
        }
    }
}
