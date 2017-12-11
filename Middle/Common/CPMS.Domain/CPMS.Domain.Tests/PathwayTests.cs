using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CPMS.Domain.Tests
{
    [TestClass]
    public class PathwayTests
    {
        [TestMethod]
        public void Validate_RaisesValidationFailedEvent_WhenPpiNumberHasALengthDifferentThan20Characters()
        {
            //Arrange
            var pathway = new Pathway
            {
                PPINumber = new string('x', 22)
            };
            RuleViolation eventRaised = null;

            pathway.ValidationFailed += delegate { eventRaised = new RuleViolation(); };

            //Act
            pathway.Validate();

            //Assert
            Assert.IsNotNull(eventRaised);
        }

        [TestMethod]
        public void Validate_DoesNotRaiseValidationFailedEvent_WhenPpiNumberHasALengthOf20Characters()
        {
            //Arrange
            var pathway = new Pathway
            {
                PPINumber = new string('x', 20)
            };
            RuleViolation eventRaised = null;

            pathway.ValidationFailed += delegate { eventRaised = new RuleViolation(); };

            //Act
            pathway.Validate();

            //Assert
            Assert.IsNull(eventRaised);
        }

        [TestMethod]
        public void Add_RaisesValidationFailedEvent_WhenThereIsAnotherActivePeriod()
        {
            //Arrange
            var pathway = new Pathway
            {
                PPINumber = "123"
            };
            pathway.Add(new RTT18WeekPeriod { Name = "p2", IsActive = false, StopDate = new DateTime(2014, 4, 3), Pathway = new Pathway { PPINumber = "ppi" } });
            pathway.Add(new RTT18WeekPeriod { Name = "p3", IsActive = false, StopDate = new DateTime(2014, 4, 3), Pathway = new Pathway { PPINumber = "ppi" } });
            pathway.Add(new RTT18WeekPeriod { Name = "p4", IsActive = true, StopDate = new DateTime(2014, 4, 3), Pathway = new Pathway { PPINumber = "ppi" } });

            RuleViolation eventRaised = null;

            pathway.ValidationFailed += delegate { eventRaised = new RuleViolation(); };
            var period = new RTT18WeekPeriod { Name = "p1", IsActive = true, Pathway = new Pathway { PPINumber = "ppi" } };

            //Act
            pathway.Add(period);

            //Assert
            Assert.IsNotNull(eventRaised);
        }

        [TestMethod]
        public void Add_DoesNotRaiseValidationFailedEvent_WhenAllPreviousPeriodsAreInactive()
        {
            //Arrange
            var pathway = new Pathway
            {
                PPINumber = "123"
            };
            pathway.Add(new RTT18WeekPeriod { Name = "p2", IsActive = false, StopDate = new DateTime(2014, 4, 3), Pathway = new Pathway { PPINumber = "ppi" } });
            pathway.Add(new RTT18WeekPeriod { Name = "p3", IsActive = false, StopDate = new DateTime(2014, 4, 3), Pathway = new Pathway { PPINumber = "ppi" } });
            pathway.Add(new RTT18WeekPeriod { Name = "p4", IsActive = false, StopDate = new DateTime(2014, 4, 3), Pathway = new Pathway { PPINumber = "ppi" } });

            RuleViolation eventRaised = null;

            pathway.ValidationFailed += delegate { eventRaised = new RuleViolation(); };
            var period = new RTT18WeekPeriod { Name = "p1", IsActive = true, Pathway = new Pathway { PPINumber = "ppi" } };

            //Act
            pathway.Add(period);

            //Assert
            Assert.IsNull(eventRaised);
        }

        [TestMethod]
        public void Add_RaisesValidationFailedEvent_WhenANonActivePeriodDoesNotHaveStopDate()
        {
            //Arrange
            var pathway = new Pathway
            {
                PPINumber = "123"
            };
            pathway.Add(new RTT18WeekPeriod { Name = "p2", IsActive = false, StopDate = new DateTime(2014, 4, 3) , Pathway = new Pathway { PPINumber = "ppi" } });
            pathway.Add(new RTT18WeekPeriod { Name = "p3", IsActive = false, Pathway = new Pathway { PPINumber = "ppi" } });
            pathway.Add(new RTT18WeekPeriod { Name = "p4", IsActive = false, StopDate = new DateTime(2014, 4, 3), Pathway = new Pathway { PPINumber = "ppi" } });

            RuleViolation eventRaised = null;

            pathway.ValidationFailed += delegate { eventRaised = new RuleViolation(); };
            var period = new RTT18WeekPeriod { Name = "p1", IsActive = true, Pathway = new Pathway { PPINumber = "ppi" } };

            //Act
            pathway.Add(period);

            //Assert
            Assert.IsNotNull(eventRaised);
        }
    }
}
