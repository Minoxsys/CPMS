namespace PAS.Models
{
    public class LitePatientViewModel
    {
        public string NHSNumber { get; set; }

        public string Name { get; set; }

        public string DisplayName { get { return string.Format("{0} (NHS:{1})", Name, NHSNumber); } }
    }
}