﻿
namespace CPMS.Trust.Presentation
{
    public interface IMapper<in TSource, out TDestination>
    {
        TDestination Map(TSource source);
    }
}