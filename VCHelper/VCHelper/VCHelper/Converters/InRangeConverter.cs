using System;
using System.Globalization;
using Xamarin.Forms;

namespace VCHelper.Converters
{
	/// <summary>
	/// Convert number to bool by range. If number in range then true.
	/// </summary>
	public class InRangeConverter : IValueConverter
	{
		/// <summary>
		/// Convert to bool
		/// </summary>
		/// <param name="value">Any number value</param>
		/// <param name="targetType"> Must be bool</param>
		/// <param name="parameter">String of range (0..1) not include extrem numbers and [0..1] - include extreme numbers</param>
		/// <returns>bool</returns>
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			try
			{
				var params_str = parameter?.ToString().Trim();
				var include = false;
				if (params_str[0] == '[')
					include = true;
				params_str = params_str.Trim('[', '(', ')', ']');
				var values = params_str.Split("..");
				var min = double.Parse(values[0]);
				var max = double.Parse(values[1]);
				var val = double.Parse(value.ToString());
				if (include)
					return (val >= min && val <= max);
				else
					return (val > min && val < max);
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine($"EXCEPTION {this?.GetType()?.Name}.Convert(value: {value}, parameter: {parameter}) : {ex.Message}\n{ex}");
				return false;
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
