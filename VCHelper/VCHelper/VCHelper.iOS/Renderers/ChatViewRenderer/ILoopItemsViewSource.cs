using Xamarin.Forms.Platform.iOS;

namespace VCHelper.iOS.Renderers.ChatViewRenderer
{
    public interface ILoopItemsViewSource : IItemsViewSource
    {
        bool Loop { get; set; }

        int LoopCount { get; }
    }
}