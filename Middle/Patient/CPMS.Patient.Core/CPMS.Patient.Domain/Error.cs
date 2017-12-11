using System;

namespace CPMS.Patient.Domain
{
    public class Error
    {
        public int Id { get; set; }

        public string Message { get; set; }

        public DateTime CreatedAt { get; set; }

        public Period Period { get; set; }
    }
}
