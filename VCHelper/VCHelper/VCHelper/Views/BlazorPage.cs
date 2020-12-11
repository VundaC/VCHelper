using Microsoft.MobileBlazorBindings;
using System.Threading.Tasks;
using VCHelper.Blazor.Pages;
using VCHelper.Blazor.Services;
using VCHelper.Razor;
using Xamarin.Forms;

namespace VCHelper.Views
{
	public class BlazorPage : ContentPage
	{
		public BlazorPage()
		{
			var commandService = DependencyService.Resolve<ICommandService>();
			commandService.SetExecution("ShowAlert", ShowAlert);
		}

		protected override void OnAppearing()
		{
			App.Host.AddComponent<Main>(this);
			base.OnAppearing();
		}

		private async Task<bool> ShowAlert(object[] args)
		{
			var title = args[0].ToString();
			var message = args[1].ToString();
			var accept = args[2].ToString();
			var cancel = args[3].ToString();

			return await DisplayAlert(title, message, accept, cancel);
		}

		protected override void OnDisappearing()
		{
			Index.Instance.Dispose();
			base.OnDisappearing();
		}
	}
}