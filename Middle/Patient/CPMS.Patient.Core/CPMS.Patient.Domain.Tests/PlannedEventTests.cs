using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CPMS.Patient.Domain.Tests
{
    [TestClass]
    public class PlannedEventTests
    {
        [TestMethod]
        public void BreachDate_ReturnsNull_WhenTargetNumberOfDaysIsNull()
        {
            // Arrange
            var plannedEvent = new PlannedEvent
            {
                TargetNumberOfDays = null,
                DateReferenceForTarget = new DateTime(2014, 7, 1),
                Id = 1
            };

            // Act
            var result = plannedEvent.BreachDate;

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void BreachDate_ReturnsNull_WhenDateReferenceForTargetIsNull()
        {
            // Arrange
            var plannedEvent = new PlannedEvent
            {
                TargetNumberOfDays = 1,
                DateReferenceForTarget = null,
                Id = 1
            };

            // Act
            var result = plannedEvent.BreachDate;

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void BreachDate_ReturnsComputedDate_WhenTargetNumberOfDaysAndDateReferenceForTargetAreNotNull()
        {
            // Arrange
            var plannedEvent = SetupPlannedEvent();

            var computedBreachDate = new DateTime(2001, 1, 1);

            // Act
            var result = plannedEvent.BreachDate;

            // Assert
            Assert.AreEqual(computedBreachDate, result);
        }

        [TestMethod]
        public void GetRemainingDays_ReturnsNull_WhenBreachDateIsNull()
        {
            // Arrange
            var plannedEvent = new PlannedEvent
            {
                TargetNumberOfDays = null,
                DateReferenceForTarget = new DateTime(2014, 7, 1),
                Id = 1
            };

            // Act
            var result = plannedEvent.GetDaysToBreachAt(new DateTime(2014, 7, 6));

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void GetRemainingDays_ReturnsComputedDaysNumber_WhenBreachDateIsNotNull()
        {
            // Arrange
            var period = new RTT18WeekPeriod();
            period.Add(new ClockStartingEvent
            {
                EventDate = new DateTime(2014, 7, 1),
                TargetDate = new DateTime(2014, 7, 1)
            });
            var @event = new ClockTickingEvent
            {
                Period = period,
                EventDate = new DateTime(2014, 7, 1)
            };
            var plannedEvent = new PlannedEvent
            {
                TargetNumberOfDays = 3,
                DateReferenceForTarget = new DateTime(2014, 7, 1),
                Id = 1,
                Event = @event
            };


            // Act
            var result = plannedEvent.GetDaysToBreachAt(new DateTime(2014, 7, 6));

            // Assert
            Assert.AreEqual(-2, result);
        }

        private static PlannedEvent SetupPlannedEvent()
        {
            var period = new RTT18WeekPeriod();
            period.Add(new ClockStartingEvent { EventDate = new DateTime(2000, 12, 25), TargetDate = new DateTime(2000, 12, 25) });
            period.Add(new ClockPausingEvent { EventDate = new DateTime(2000, 12, 27) });
            period.Add(new ClockStartingEvent { EventDate = new DateTime(2000, 12, 29), TargetDate = new DateTime(2000, 12, 30) });

            return new PlannedEvent
            {
                TargetNumberOfDays = 5,
                DateReferenceForTarget = new DateTime(2000,12, 25),
                Id = 1,
                Event = new ClockTickingEvent
                {
                    Period = period,
                    EventDate = new DateTime(2000, 12, 31),TargetDate = new DateTime(2001, 1, 4)
                }
            };
        }

    }
}
