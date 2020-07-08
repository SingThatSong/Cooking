using Cooking.ServiceLayer;
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

                        if (valueInt > Consts.YearDays)
                        {
                            return localization.GetLocalizedString("YearsAgo", valueInt / Consts.YearDays);
                        }

                        if (valueInt > Consts.MonthDays)
                        {
                            return localization.GetLocalizedString("MonthsAgo", valueInt / Consts.MonthDays);
                        }

                        if (valueInt > Consts.WeekDays)
                        {
                            return localization.GetLocalizedString("WeeksAgo", valueInt / Consts.WeekDays);
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
