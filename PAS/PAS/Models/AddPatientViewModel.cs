using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PAS.Models
{
    public class AddPatientViewModel
    {
        [Required]
        public PatientTitle Title { get; set; }

        [Required, MaxLength(50)]
        [RegularExpression(@"^[0-9A-Za-z ]+$", ErrorMessage = "Special characters should not be entered.")]
        public string Name { get; set; }

        [Required]
        public DateTime? DateOfBirth { get; set; }

        [Required, MinLength(10, ErrorMessage = "Length must be of exactly 10 characters."), MaxLength(10, ErrorMessage = "Length must be of exactly 10 characters.")]
        [RegularExpression(@"^[0-9A-Za-z]+$", ErrorMessage = "Special characters should not be entered.")]
        public string NHSNumber { get; set; }

        [RegularExpression(@"^[0-9A-Za-z ]+$", ErrorMessage = "Special characters should not be entered.")]
        [MaxLength(50)]
        public string ConsultantName { get; set; }

        [MaxLength(20)]
        [RegularExpression(@"^[0-9A-Za-z]+$", ErrorMessage = "Special characters should not be entered.")]
        public string ConsultantNumber { get; set; }

        public IEnumerable<PatientViewModel> AllPatients { get; set; }
    }
}