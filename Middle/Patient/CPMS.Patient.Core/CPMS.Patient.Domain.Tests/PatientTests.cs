using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CPMS.Patient.Domain.Tests
{
    [TestClass]
    public class PatientTests
    {
        [TestMethod]
        public void GetAge_ReturnsCorectValue_WhenTodayDayInMonthIsGreaterThanBirthDayInMonth()
        {
            //Arrange
            var patient = new Patient
            {
                Name = "John Doe",
                DateOfBirth = new DateTime(1991, 4, 11)
            };

            //Act
            var age = patient.GetAgeAt(new DateTime(2014, 4, 15));

            //Assert
            Assert.AreEqual(23, age);
        }

        [TestMethod]
        public void GetAge_ReturnsCorectValue_WhenTodayDayInMonthIsSmallerThanBirthDayInMonth()
        {
            //Arrange
            var patient = new Patient
            {
                Name = "John Doe",
                DateOfBirth = new DateTime(1991, 4, 15)
            };

            //Act
            var age = patient.GetAgeAt(new DateTime(2014, 4, 11));

            //Assert
            Assert.AreEqual(22, age);
        }

        [TestMethod]
        public void Validate_RaisesValidationFailedEvent_WhenNhsNumberHasALengthDifferentThan10Characters()
        {
            //Arrange
            var patient = new Patient
            {
                Name = "John Doe",
                NHSNumber = new string('x', 22)
            };
            var eventRaised = false;

            patient.ValidationFailed += delegate { eventRaised = true; };

            //Act
            patient.Validate();

            //Assert
            Assert.IsTrue(eventRaised);
        }

        [TestMethod]
        public void Validate_DoesNotRaiseValidationFailedEvent_WhenNhsNumberHasALengthOf10Characters()
        {
            //Arrange
            var patient = new Patient
            {
                Name = "John Doe",
                NHSNumber = new string('x', 10)
            };
            var eventRaised = false;

            patient.ValidationFailed += delegate { eventRaised = true; };

            //Act
            patient.Validate();

            //Assert
            Assert.IsFalse(eventRaised);
        }

        [TestMethod]
        public void IsChild_ReturnsTrue_WhenPatientAgeLessThan18()
        {
            // Arrange
            var patient = new Patient
            {
                DateOfBirth = new DateTime(2000, 3, 4)
            };

            // Act
            var result = patient.IsChild(new DateTime(2014, 4, 3));

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void IsChild_ReturnsTrue_WhenPatientAgeHigherThan18()
        {
            // Arrange
            var patient = new Patient
            {
                DateOfBirth = new DateTime(1995, 3, 4)
            };

            // Act
            var result = patient.IsChild(new DateTime(2014, 4, 3));

            // Assert
            Assert.IsFalse(result);
        }
    }
}
