using System;
using System.ComponentModel;
using System.Reflection;

namespace Cooking.WPF.Services
{
    public static class EnumExtensions
    {
        /// <summary>
        /// Get DescriptionAttribute from enum
        /// </summary>
        public static string? Description(this Enum value)
        {
            Type enumType = value.GetType();
            FieldInfo? field = enumType.GetField(value.ToString());

            if (field == null)
            {
                return null;
            }

            object[] attributes = field.GetCustomAttributes(typeof(DescriptionAttribute), false);
            return attributes.Length == 0
                ? value.ToString()
                : ((DescriptionAttribute)attributes[0]).Description;
        }

        /// <summary>
        /// Get Enum value from Description
        /// </summary>
        public static Enum? Enum(this Type type, string description)
        {
            Array values = System.Enum.GetValues(type);

            foreach (object? value in values)
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
