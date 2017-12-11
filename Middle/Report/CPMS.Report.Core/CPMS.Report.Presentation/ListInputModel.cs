namespace CPMS.Report.Presentation
{
    public class ListInputModel
    {
        public int? PageCount { get; set; }

        public int? Index { get; set; }

        public string OrderBy { get; set; }

        public string OrderDirection { get; set; }
    }
}
