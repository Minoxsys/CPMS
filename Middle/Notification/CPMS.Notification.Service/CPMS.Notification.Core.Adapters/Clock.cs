using System;
using System.Configuration;
using CPMS.Patient.Domain;

namespace CPMS.Notification.Core.Adapters
{
    public class Clock : IClock
    {
        public DateTime TodayDate
        {
            get
            {
                DateTime todayDate;

                return DateTime.TryParse(ConfigurationManager.AppSettings["UtcNow"], out todayDate) ? todayDate.Date : DateTime.UtcNow.Date;
            }
        }

        public DateTime TodayDateAndTime
        {
            get
            {
                DateTime todayDate;

                return DateTime.TryParse(ConfigurationManager.AppSettings["UtcNow"], out todayDate) ? todayDate : DateTime.UtcNow;
            }
        }
    }
}
