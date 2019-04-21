﻿using Cooking.Helpers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;

namespace Cooking.Converters
{
    public class EnumToDescriptionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Enum @enum)
            {
                return @enum.Description();
            }
            else if (value is IEnumerable collection)
            {
                List<string> list = new List<string>();

                foreach(var val in collection)
                {
                    if(val is Enum @enumValue)
                    {
                        list.Add(@enumValue.Description());
                    }
                }

                return list;
            }
            else
            {
                return value;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return targetType.Enum(value.ToString());
        }
    }
}
