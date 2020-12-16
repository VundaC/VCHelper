using System;
using System.Globalization;
using Xamarin.Forms;

namespace VCHelper.Converters
{
	/// <summary>
	/// Convert bool value
	/// </summary>
	public class BoolInvertConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is bool b) return !b;
			throw new InvalidCastException($"Excpect bool insead of {value?.GetType()?.Name}");
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is bool b) return !b;
			throw new InvalidCastException($"Excpect bool insead of {value?.GetType()?.Name}");
		}
	}
}
