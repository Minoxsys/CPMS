using System;

namespace CPMS.Patient.Domain
{
    public class Patient : ValidationBase
    {
        private const int Length = 10;

        public string NHSNumber { get; set; }

        public string Title { get; set; }

        public string Name { get; set; }

        public DateTime DateOfBirth { get; set; }

        public string ConsultantName { get; set; }

        public string ConsultantNumber { get; set; }

        public int GetAgeAt(DateTime date)
        {
            var age = date.Year - DateOfBirth.Year;

            return (date.DayOfYear < DateOfBirth.DayOfYear) ? --age : age;
        }

        public bool IsChild(DateTime date)
        {
            return GetAgeAt(date) < 18;
        }

        public void Validate()
        {
            if (NHSNumber.Length != Length)
            {
                OnValidationFailed(new Error
                {
                    Message = string.Format("Nhs number {0} should have exactly {1} characters.", NHSNumber, Length)
                });
            }
        }
    }
}
