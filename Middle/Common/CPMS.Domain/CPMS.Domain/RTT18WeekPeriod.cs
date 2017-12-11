using System;

namespace CPMS.Domain
{
    public class RTT18WeekPeriod : Period
    {
        private int? _maximumNumberOfDays;
        private bool _shouldCountForBreaches;

        public override int MaximumNumberOfDays
        {
            get
            {
                _maximumNumberOfDays = 126;

                return _maximumNumberOfDays.Value;
            }

            set { _maximumNumberOfDays = value; }
        }

        public override bool ShouldCountForBreaches
        {
            get
            {
                _shouldCountForBreaches = true;
                return _shouldCountForBreaches;
            }
            set { _shouldCountForBreaches = value; }
        }
        public override string  GetPauseValidationMessage{ get { return string.Empty; } }

        public override bool AbleToNotAttendFirstEvent { get { return false; } }

        public override int GetDaysRemainingAt(DateTime dateTime)
        {
            return MaximumNumberOfDays - GetDaysSpentAt(dateTime);
        }

    }
}
