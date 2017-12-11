using System;
using System.ComponentModel;
using System.Reflection;

namespace CPMS.Patient.Delivery.Adapters
{
    public static class EnumExtensions
    {
        public static string GetDescription<TEnum>(this TEnum value)
        {
            var info = value.GetType().GetField(value.ToString());

            var attribute = info.GetCustomAttribute(typeof(DescriptionAttribute), false) as DescriptionAttribute;

            if (attribute != null)
            {
                return attribute.Description;
            }

            return value.ToString();
        }

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