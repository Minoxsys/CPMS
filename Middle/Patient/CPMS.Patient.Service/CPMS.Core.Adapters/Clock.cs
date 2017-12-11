using System;
using System.Configuration;
using CPMS.Patient.Domain;

namespace CPMS.Core.Adapters
{
    public class Clock : IClock
    {
       public DateTime TodayDate
        {
            get
            {
                DateTime todayDate;

                //return DateTime.TryParse(ConfigurationManager.AppSettings["UtcNow"], out todayDate) ? todayDate.Date : DateTime.UtcNow.Date;

                return new DateTime(2016, 4, 20);
            }
        }

        public DateTime TodayDateAndTime
        {
            get
            {
                DateTime todayDate;

                //return DateTime.TryParse(ConfigurationManager.AppSettings["UtcNow"], out todayDate) ? todayDate : DateTime.UtcNow;

                return new DateTime(2016, 4, 20);
            }
        }
    }
}
