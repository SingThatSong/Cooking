using Cooking.ServiceLayer;
using Cooking.WPF.Services;
using NullGuard;
using Prism.Ioc;
using Prism.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace Cooking.WPF.Converters
{
    /// <summary>
    /// Converter for displaying <see cref="System.ComponentModel.DescriptionAttribute"/> values from enums.
    /// </summary>
    public class EnumToDescriptionConverter : IValueConverter
    {
        /// <inheritdoc/>
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (Application.Current is PrismApplication prismApplication)
            {
                ILocalization localization = prismApplication.Container.Resolve<ILocalization>();

                if (value is Enum @enum)
                {
                    return localization[@enum];
                }
                else if (value is IEnumerable collection and not string)
                {
                    var list = new List<string>();

                    foreach (object? val in collection)
                    {
                        if (val is Enum @enumValue)
                        {
                            string? description = localization[@enumValue];
                            if (description != null)
                            {
                                list.Add(description);
                            }
                        }
                    }

                    return list;
                }
            }

            return value;
        }

        /// <inheritdoc/>
        public object? ConvertBack(object? value, Type? targetType, object? parameter, CultureInfo culture)
        {
            if (Application.Current is PrismApplication prismApplication)
            {
                ILocalization localization = prismApplication.Container.Resolve<ILocalization>();

                if (value != null && targetType != null)
                {
                    if (targetType.IsGenericType && targetType.GetGenericTypeDefinition() == typeof(Nullable<>))
                    {
                        targetType = targetType.GetGenericArguments()[0];
                    }

                    Dictionary<string, string> allValues = localization.GetAllValuesFor(targetType.Name);
                    string? valAsString = value.ToString();
                    string key = allValues.FirstOrDefault(x => x.Value == valAsString).Key;
                    return Enum.Parse(targetType, key);
                }
            }

            return Binding.DoNothing;
        }
    }
}
