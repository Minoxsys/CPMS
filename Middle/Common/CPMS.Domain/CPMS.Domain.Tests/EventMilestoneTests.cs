using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CPMS.Domain.Tests
{
    [TestClass]
    public class EventMilestoneTests
    {
        [TestMethod]
        public void BreachDate_ReturnsNull_WhenTargetNumberOfDaysIsNull()
        {
            // Arrange
            var eventMilestone = new EventMilestone
            {
                TargetNumberOfDays = null,
                DateReferenceForTarget = new DateTime(2014, 7, 1),
                Id = 1
            };

            // Act
            var result = eventMilestone.BreachDate;

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void BreachDate_ReturnsNull_WhenDateReferenceForTargetIsNull()
        {
            // Arrange
            var eventMilestone = new EventMilestone
            {
                TargetNumberOfDays = 1,
                DateReferenceForTarget = null,
                Id = 1
            };

            // Act
            var result = eventMilestone.BreachDate;

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void BreachDate_ReturnsComputedDate_WhenTargetNumberOfDaysAndDateReferenceForTargetAreNotNull()
        {
            // Arrange
            var eventMilestone = SetupEventMilestone();

            var computedBreachDate = new DateTime(2001, 1, 1);

            // Act
            var result = eventMilestone.BreachDate;

            // Assert
            Assert.AreEqual(computedBreachDate, result);
        }

        [TestMethod]
        public void GetRemainingDays_ReturnsNull_WhenBreachDateIsNull()
        {
            // Arrange
            var eventMilestone = new EventMilestone
            {
                TargetNumberOfDays = null,
                DateReferenceForTarget = new DateTime(2014, 7, 1),
                Id = 1
            };

            // Act
            var result = eventMilestone.GetDaysToBreachAt(new DateTime(2014, 7, 6));

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void GetRemainingDays_ReturnsComputedDaysNumber_WhenBreachDateIsNotNull()
        {
            // Arrange
            var period = new RTT18WeekPeriod();
            period.Add(new ClockStartingCompletedEvent
            {
                EventDate = new DateTime(2014, 7, 1),
                TargetDate = new DateTime(2014, 7, 1)
            });
            var @event = new ClockTickingCompletedEvent
            {
                Period = period,
                EventDate = new DateTime(2014, 7, 1)
            };
            var eventMilestone = new EventMilestone
            {
                TargetNumberOfDays = 3,
                DateReferenceForTarget = new DateTime(2014, 7, 1),
                Id = 1,
                CompletedEvent = @event
            };


            // Act
            var result = eventMilestone.GetDaysToBreachAt(new DateTime(2014, 7, 6));

            // Assert
            Assert.AreEqual(-2, result);
        }

        private static EventMilestone SetupEventMilestone()
        {
            var period = new RTT18WeekPeriod();
            period.Add(new ClockStartingCompletedEvent { EventDate = new DateTime(2000, 12, 25), TargetDate = new DateTime(2000, 12, 25) });
            period.Add(new ClockPausingCompletedEvent { EventDate = new DateTime(2000, 12, 27) });
            period.Add(new ClockStartingCompletedEvent { EventDate = new DateTime(2000, 12, 29), TargetDate = new DateTime(2000, 12, 30) });

            return new EventMilestone
            {
                TargetNumberOfDays = 5,
                DateReferenceForTarget = new DateTime(2000,12, 25),
                Id = 1,
                CompletedEvent = new ClockTickingCompletedEvent
                {
                    Period = period,
                    EventDate = new DateTime(2000, 12, 31),TargetDate = new DateTime(2001, 1, 4)
                }
            };
        }

    }
}
