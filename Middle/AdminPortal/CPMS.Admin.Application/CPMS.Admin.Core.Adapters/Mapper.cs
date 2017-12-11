using AutoMapper;

namespace CPMS.Admin.Core.Adapters
{
    public class Mapper<TSource, TDestination>: Application.IMapper<TSource, TDestination>, Presentation.IMapper<TSource,TDestination>
    {
        public TDestination Map(TSource source)
        {
            return Mapper.Map<TSource, TDestination>(source);
        }
    }
}
