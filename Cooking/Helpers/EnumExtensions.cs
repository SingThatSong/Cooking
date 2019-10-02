using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cooking.Helpers
{
    public static class EnumExtensions
    {
        /// <summary>
        /// Get DescriptionAttribute from enum
        /// </summary>
        public static string Description(this Enum value)
        {
            var enumType = value.GetType();
            var field = enumType.GetField(value.ToString());

            if (field == null)
            {
                return null;
            }

            var attributes = field.GetCustomAttributes(typeof(DescriptionAttribute), false);
            return attributes.Length == 0
                ? value.ToString()
                : ((DescriptionAttribute)attributes[0]).Description;
        }

        /// <summary>
        /// Get Enum value from Description
        /// </summary>
        public static Enum? Enum(this Type type, string description)
        {
            var values = System.Enum.GetValues(type);

            foreach (var value in values)
            {
                if (description == Description((Enum)value!))
                {
                    return (Enum)value!;
                }
            }

            return null;
        }
    }
}
