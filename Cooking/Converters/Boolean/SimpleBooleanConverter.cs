using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Cooking.Converters
{
    public class SimpleBooleanConverter : BooleanConverter<bool>
    {
        public SimpleBooleanConverter() : base(true, false)
        {

        }

        public SimpleBooleanConverter(bool trueValue, bool falseValue) : base(trueValue, falseValue)
        {
        }
    }
}
