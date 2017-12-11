using System;

namespace CPMS.Domain
{
    public class Non18WeekPeriod : Period
    {
        private int? _maximumNumberOfDays;
        private bool _shouldCountForBreaches;

        public override int MaximumNumberOfDays
        {
            get
            {
                _maximumNumberOfDays = 0;
                return _maximumNumberOfDays.Value;
            }
            set
            {
                _maximumNumberOfDays = value;
            }
        }

        public override bool ShouldCountForBreaches
        {
            get
            {
                _shouldCountForBreaches = false;
                return _shouldCountForBreaches;
            }
            set { _shouldCountForBreaches = value; }
        }

        public override string GetPauseValidationMessage { get { return "should not pause a Non 18 week period."; } }

        public override bool AbleToNotAttendFirstEvent { get { return false; } }

        public override int GetDaysRemainingAt(DateTime dateTime)
        {
            return 0;
        }
    }
}
