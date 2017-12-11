using AutoMapper;

namespace CPMS.Report.Core.Adapters
{
    public class Mapper<TSource, TDestination> : Presentation.IMapper<TSource, TDestination>
    {
        public TDestination Map(TSource source)
        {
            return Mapper.Map<TSource, TDestination>(source);
        }
    }
}
