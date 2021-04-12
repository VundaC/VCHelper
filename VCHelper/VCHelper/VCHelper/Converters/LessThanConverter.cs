using System;
using System.Globalization;
using Xamarin.Forms;

namespace VCHelper.Converters
{
    public class LessThanConverter : IValueConverter, IMultiValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!double.TryParse($"{value}", out var dVal) || !double.TryParse($"{parameter}", out var dParam))
            {
                return false;
            }
            return dVal > dParam;
        }

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length != 2)
            {
                throw new ArgumentException();
            }

            if (!double.TryParse($"{values[0]}", out var dVal) || !double.TryParse($"{values[1]}", out var dParam))
            {
                return false;
            }
            return dVal > dParam;
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
