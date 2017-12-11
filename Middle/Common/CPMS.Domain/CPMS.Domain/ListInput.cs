namespace CPMS.Domain
{
    public class ListInput
    {
        public int? PageCount { get; set; }

        public int? Index { get; set; }

        public OrderBy? OrderBy { get; set; }

        public OrderDirection? OrderDirection { get; set; }
    }
}
