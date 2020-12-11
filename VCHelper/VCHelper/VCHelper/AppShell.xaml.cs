using VCHelper.Views;
using Xamarin.Forms;

namespace VCHelper
{
	public partial class AppShell : Shell
	{
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(HomePage), typeof(HomePage));
            Routing.RegisterRoute(nameof(BlazorPage), typeof(BlazorPage));
        }
    }
}
