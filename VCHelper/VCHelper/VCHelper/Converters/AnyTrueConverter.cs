using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Xamarin.Forms;

namespace VCHelper.Converters
{
    public class AnyTrueConverter : IValueConverter, IMultiValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not IEnumerable<bool> enumerable)
            {
                return false;
            }
            return enumerable.Any(x => x);
        }

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            //return true if all values are boolean and equals to true
            return values.Any(x => x is bool b && b);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
