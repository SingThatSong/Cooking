using Cooking.WPF.Helpers;
using Prism.Ioc;
using Prism.Unity;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Cooking.WPF.Converters
{
    public class LastCookedConverter : IValueConverter
    {
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

        // According to https://msdn.microsoft.com/en-us/library/system.windows.data.ivalueconverter.convertback(v=vs.110).aspx#Anchor_1
        // (kudos Scott Chamberlain), if you do not support a conversion
        // back you should return a Binding.DoNothing or a
        // DependencyProperty.UnsetValue
        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => Binding.DoNothing;
    }
}
