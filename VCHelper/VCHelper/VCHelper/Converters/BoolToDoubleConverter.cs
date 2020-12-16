using System;
using System.Globalization;
using Xamarin.Forms;

namespace VCHelper.Converters
{
	class BoolToDoubleConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (!(value is bool b)) throw new InvalidCastException($"Excpect bool insead of {value?.GetType()?.Name}");
			var parameters = parameter?.ToString().Split(':');
			if (parameters.Length != 2) throw new InvalidCastException("Conver parameter must be like 'tureValue:falseValue'");
			return double.Parse(parameters[b ? 0 : 1], CultureInfo.InvariantCulture);
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
