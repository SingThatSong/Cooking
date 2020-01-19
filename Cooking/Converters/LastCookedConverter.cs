using Cooking.WPF.Helpers;
using Prism.Unity;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using Prism.Ioc;

namespace Cooking.WPF.Converters
{
    public class LastCookedConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object? parameter, CultureInfo culture)
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
                            string? yearsAgoCaption = localization.GetLocalizedString("YearsAgo");
                            if (yearsAgoCaption != null)
                            {
                                int yearsAgo = valueInt / 365;
                                return string.Format(localization.CurrentCulture, yearsAgoCaption, yearsAgo);
                            }
                        }

                        if (valueInt > 30)
                        {
                            string? monthsAgoCaption = localization.GetLocalizedString("MonthsAgo");
                            if (monthsAgoCaption != null)
                            {
                                int monthsAgo = valueInt / 30;
                                return string.Format(localization.CurrentCulture, monthsAgoCaption, monthsAgo);
                            }
                        }


                        string? daysAgoCaption = localization.GetLocalizedString("DaysAgo");
                        if (daysAgoCaption != null)
                        {
                            return string.Format(localization.CurrentCulture, daysAgoCaption, valueInt);
                        }
                    }
                }
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // According to https://msdn.microsoft.com/en-us/library/system.windows.data.ivalueconverter.convertback(v=vs.110).aspx#Anchor_1
            // (kudos Scott Chamberlain), if you do not support a conversion 
            // back you should return a Binding.DoNothing or a 
            // DependencyProperty.UnsetValue
            return Binding.DoNothing;
        }
    }
}
