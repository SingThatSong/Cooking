using Cooking.WPF.Services;
using Prism.Ioc;
using Prism.Unity;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Cooking.WPF.Converters
{
    /// <summary>
    /// Convert LastCooked value to localized string.
    /// </summary>
    public class LastCookedConverter : IValueConverter
    {
        /// <inheritdoc/>
        public object? Convert(object value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is int valueInt)
            {
                if (valueInt == int.MaxValue)
                {
                    return value;
                }
                else
                {
                    if (Application.Current is PrismApplication prismApplication)
                    {
                        ILocalization localization = prismApplication.Container.Resolve<ILocalization>();

                        if (valueInt > 365)
                        {
                            return localization.GetLocalizedString("YearsAgo", valueInt / 365);
                        }

                        if (valueInt > 30)
                        {
                            return localization.GetLocalizedString("MonthsAgo", valueInt / 30);
                        }

                        return localization.GetLocalizedString("DaysAgo", valueInt);
                    }
                }
            }

            return value;
        }

        /// <inheritdoc/>
        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => Binding.DoNothing;
    }
}
