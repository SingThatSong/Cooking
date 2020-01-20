using Cooking.WPF.Helpers;
using Cooking.WPF.Services;
using NullGuard;
using Prism.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using Prism.Ioc;
using System.Linq;

namespace Cooking.WPF.Converters
{
    /// <summary>
    /// Converter for displaying <see cref="System.ComponentModel.DescriptionAttribute"/> values from enums.
    /// </summary>
    public class EnumToDescriptionConverter : IValueConverter
    {
        /// <inheritdoc/>
        public object? Convert(object value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (Application.Current is PrismApplication prismApplication)
            {
                ILocalization localization = prismApplication.Container.Resolve<ILocalization>();

                if (value is Enum @enum)
                {
                    return localization.GetLocalizedString(@enum);
                }
                else if (value is IEnumerable collection)
                {
                    var list = new List<string>();

                    foreach (object? val in collection)
                    {
                        if (val is Enum @enumValue)
                        {
                            string? description = localization.GetLocalizedString(@enumValue);
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
        public object? ConvertBack([AllowNull] object? value, Type? targetType, [AllowNull] object? parameter, CultureInfo culture)
        {
            if (Application.Current is PrismApplication prismApplication)
            {
                ILocalization localization = prismApplication.Container.Resolve<ILocalization>();

                if (value != null && targetType != null)
                {
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
