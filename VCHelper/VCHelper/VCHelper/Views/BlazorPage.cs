using Microsoft.MobileBlazorBindings;
using VCHelper.Razor;
using Xamarin.Forms;

namespace VCHelper.Views
{
	public class BlazorPage : ContentPage
	{
		public BlazorPage()
		{
			App.Host.AddComponent<Main>(this);
		}
	}
}