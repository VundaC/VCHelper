using VCHelper.Controls.AnimatedNavigationPage;
using VCHelper.iOS.Renderers.AnimatedNavigationPage;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(AnimatedNavigationPage), typeof(AnimatedNavigationPageRenderer_iOS))]
namespace VCHelper.iOS.Renderers.AnimatedNavigationPage
{
    public class AnimatedNavigationPageRenderer_iOS : NavigationRenderer
    {
        public AnimatedNavigationPageRenderer_iOS()
        {
            Delegate = new AnimatedNavigationPageRendererNavigationDelegate(Delegate);
        }
    }
}