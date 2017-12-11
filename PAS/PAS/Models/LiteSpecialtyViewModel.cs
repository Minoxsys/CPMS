
namespace PAS.Models
{
    public class LiteSpecialtyViewModel
    {
        public string Code { get; set; }

        public string Name { get; set; }

        public string DisplayName
        {
            get { return Code == "0" ? Name : string.Format("{0} (Code:{1})", Name, Code); }
        }
    }
}
