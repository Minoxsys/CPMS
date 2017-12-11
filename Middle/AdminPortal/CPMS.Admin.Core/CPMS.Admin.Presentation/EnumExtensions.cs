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
    }
}
