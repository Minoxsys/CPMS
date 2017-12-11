using AutoMapper;

namespace CPMS.Core.Adapters
{
    public class Mapper<TSource, TDestination> : Patient.Manager.IMapper<TSource, TDestination>, Patient.Presentation.IMapper<TSource, TDestination>
    {
        public TDestination Map(TSource source)
        {
            return Mapper.Map<TSource, TDestination>(source);
        }
    }
}
