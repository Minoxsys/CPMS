using AutoMapper;

namespace CPMS.User.Core.Adapters
{
    public class Mapper<TSource, TDestination> : Presentation.IMapper<TSource, TDestination>, Manager.IMapper<TSource, TDestination>
    {
        public TDestination Map(TSource source)
        {
            return Mapper.Map<TSource, TDestination>(source);
        }
    }
}
