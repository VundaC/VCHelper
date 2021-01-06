using ObjCRuntime;
using UIKit;

namespace VCHelper.iOS.Renderers.AnimatedNavigationPage
{
    public class AnimatedNavigationPageRendererNavigationDelegate : UINavigationControllerDelegate
    {
        readonly IUINavigationControllerDelegate OldDelegate;

        public AnimatedNavigationPageRendererNavigationDelegate(IUINavigationControllerDelegate oldDelegate)
        {
            OldDelegate = oldDelegate;
        }

        public override void DidShowViewController(UINavigationController navigationController, [Transient] UIViewController viewController, bool animated)
        {
            OldDelegate?.DidShowViewController(navigationController, viewController, animated);
        }

        public override void WillShowViewController(UINavigationController navigationController, [Transient] UIViewController viewController, bool animated)
        {
            OldDelegate?.WillShowViewController(navigationController, viewController, animated);
        }

        public override IUIViewControllerAnimatedTransitioning GetAnimationControllerForOperation(UINavigationController navigationController, UINavigationControllerOperation operation, UIViewController fromViewController, UIViewController toViewController)
        {
            return new AnimatedNavigationPageTransitioning(operation);
        }
    }
}