using UIKit;

namespace VCHelper.iOS.Renderers.ChatViewRenderer
{
    public static class VersionsExtensions
    {
        public static bool IsiOS11OrNewer => UIDevice.CurrentDevice.CheckSystemVersion(11, 0);
        public static bool IsiOS13OrNewer => UIDevice.CurrentDevice.CheckSystemVersion(13, 0);
    }
}