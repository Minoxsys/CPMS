using AutoMapper;
using CPMS.Notification.Presentation;

namespace CPMS.Notification.Core.Adapters
{
    public class Mapper<TSource, TDestination> : IMapper<TSource, TDestination>
    {
        public TDestination Map(TSource source)
        {
            return Mapper.Map<TSource, TDestination>(source);
        }
    }
}
