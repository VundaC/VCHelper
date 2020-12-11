using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace VCHelper.Converters
{
    public class UrlImageSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string url)
                return UriImageSource.FromUri(new Uri(url));
            throw new ArgumentException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is UriImageSource source)
                return source.Uri.AbsolutePath;
            throw new ArgumentException();
        }
    }
}
