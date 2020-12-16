using System;
using System.Globalization;
using Xamarin.Forms;

namespace VCHelper.Converters
{
	public class MultiplyConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (parameter == null || value == null) return value;
			try
			{
				var factor = double.Parse(parameter.ToString());
				if (value is int i) return (int)(i * factor);
				if (value is double d) return d * factor;
				if (value is decimal dec) return dec * (decimal)factor;
				throw new InvalidCastException($"need implement for type: {value?.GetType()}");
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine($"EXCEPTION {this?.GetType()?.Name}.Convert() : {ex.Message}\n{ex}");
				return value;
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (parameter == null || value == null) return value;
			try
			{
				var factor = double.Parse(parameter.ToString());
				if (value is int i) return (int)(i / factor);
				if (value is double d) return d / factor;
				if (value is decimal dec) return dec / (decimal)factor;
				throw new InvalidCastException($"need implement for type: {value?.GetType()}");
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine($"EXCEPTION {this?.GetType()?.Name}.Convert() : {ex.Message}\n{ex}");
				return value;
			}
		}
	}
}
