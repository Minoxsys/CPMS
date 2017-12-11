﻿using System;
using System.Linq;

namespace CPMS.Patient.Domain
{
    public class CancerPeriod : Period
    {
        private int? _maximumNumberOfDays;
        private bool _shouldCountForBreaches;

        public override int MaximumNumberOfDays
        {
            get
            {
                //var diagnosticTestResultAvailableEvent =
                //    Events.FirstOrDefault(ev => ev.Code == EventCode.DiagnosticsTestResultAvailable);

                //_maximumNumberOfDays = diagnosticTestResultAvailableEvent != null ? 31 : 62;

                //return _maximumNumberOfDays.Value;
                return 31;
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

        public override string  GetPauseValidationMessage{ get {return "should not pause a Cancer period.";} }

        public override bool AbleToNotAttendFirstEvent { get { return true; } }

        public override int GetDaysRemainingAt(DateTime dateTime)
        {
            //var diagnosticTestResultAvailableEvent =
            //    Events.FirstOrDefault(ev => ev.Code == EventCode.DiagnosticsTestResultAvailable);

            //var daysForEntirePeriod = diagnosticTestResultAvailableEvent != null
            //    ? GetDaysSpentAt(diagnosticTestResultAvailableEvent.EventDate) + MaximumNumberOfDays
            //    : MaximumNumberOfDays;

            var daysForEntirePeriod = MaximumNumberOfDays;

            return daysForEntirePeriod - GetDaysSpentAt(dateTime);
        }
    }
}
