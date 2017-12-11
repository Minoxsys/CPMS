using AutoMapper;

namespace CPMS.Trust.Core.Adapters
{
    public class Mapper<TSource, TDestination> : Manager.IMapper<TSource, TDestination>, Presentation.IMapper<TSource, TDestination>
    {
        public TDestination Map(TSource source)
        {
            return Mapper.Map<TSource, TDestination>(source);
        }
    }
}
