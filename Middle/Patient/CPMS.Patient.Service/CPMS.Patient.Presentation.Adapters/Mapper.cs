using AutoMapper;

namespace CPMS.Patient.Presentation.Adapters
{
    public class Mapper<TSource, TDestination>: IMapper<TSource, TDestination>
    {
        public TDestination Map(TSource source)
        {
            return Mapper.Map<TSource, TDestination>(source);
        }
    }
}
