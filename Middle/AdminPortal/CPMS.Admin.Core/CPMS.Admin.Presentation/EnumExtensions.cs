using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace CPMS.Admin.Presentation
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

        public static List<TEnum> GetValuesLikeDescription<TEnum>(string description)
        {
            var type = typeof(TEnum);
            var values = new List<TEnum>();

            foreach (var field in type.GetFields())
            {
                var attribute = field.GetCustomAttribute(typeof(DescriptionAttribute)) as DescriptionAttribute;
                if (attribute != null)
                {
                    if (attribute.Description.ToLowerInvariant().Contains(description.ToLowerInvariant()))
                    {
                        values.Add((TEnum)field.GetValue(null));
                    }
                }
            }

            return values;
        }
    }
}
