using System;
using System.ComponentModel;
using System.Reflection;

namespace CPMS.Report.Delivery.Adapters
{
    public static class EnumExtensions
    {
        public static TEnum GetValueFromDescription<TEnum>(string description)
        {
            var type = typeof(TEnum);

            foreach (var field in type.GetFields())
            {
                var attribute = field.GetCustomAttribute(typeof(DescriptionAttribute)) as DescriptionAttribute;
                if (attribute != null)
                {
                    if (attribute.Description == description)
                        return (TEnum)field.GetValue(null);
                }
            }

            throw new ArgumentException();
        }
    }
}
