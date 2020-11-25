using Xamarin.Forms;

namespace VCHelper
{
    public partial class App : Application
    {

        public App()
        {
            InitializeComponent();

            //Register services here
            //DependencyService.Register<MockDataStore>();

            MainPage = new AppShell();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
