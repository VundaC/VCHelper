using System;
using System.Collections;
using System.Globalization;
using Xamarin.Forms;

namespace VCHelper.Converters
{
	public class IsNullOrEmptyConverter : IValueConverter
	{
		/// <summary>
		/// Convert string/enumerable/object type to bool
		/// </summary>
		/// <param name="value">string, object, list, observable collection etc</param>
		/// <param name="targetType">bool</param>
		/// <param name="culture"></param>
		/// <returns>bool</returns>
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value == null)
				return true;
			if (value is string str)
				return string.IsNullOrEmpty(str);
			if (value is IEnumerable enumerable)
			{
				var enumerator = enumerable.GetEnumerator();
				return !enumerator.MoveNext();
			}
			return false;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
