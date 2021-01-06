using CoreGraphics;
using System.Linq;
using System.Threading.Tasks;
using UIKit;
using VCHelper.Controls.AnimatedNavigationPage;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

namespace VCHelper.iOS.Renderers.AnimatedNavigationPage
{
    public class AnimatedNavigationPageTransitioning : UIViewControllerAnimatedTransitioning
    {
        private readonly UINavigationControllerOperation _operation;
        public AnimatedNavigationPageTransitioning(UINavigationControllerOperation operation)
        {
            _operation = operation;
        }

        public override async void AnimateTransition(IUIViewControllerContextTransitioning transitionContext)
        {
            var containerView = transitionContext.ContainerView;

            var fromController = transitionContext.GetViewControllerForKey(UITransitionContext.FromViewControllerKey);
            var toController = transitionContext.GetViewControllerForKey(UITransitionContext.ToViewControllerKey);
            var fromPage = GetPageFromController(fromController);
            var toPage = GetPageFromController(toController);

            var duration = TransitionDuration(transitionContext);

            containerView.InsertSubview(toController.View, 0);
            var isPush = _operation == UINavigationControllerOperation.Push;
            if (isPush)
                await Task.Yield();

            containerView.BringSubviewToFront(toController.View);

            var enterAnimation = isPush ?
                VCHelper.Controls.AnimatedNavigationPage.AnimatedNavigationPage.GetEnterAnimation(toPage) :
                VCHelper.Controls.AnimatedNavigationPage.AnimatedNavigationPage.GetReturnAnimation(toPage);

            var exitAnimation = isPush ?
                VCHelper.Controls.AnimatedNavigationPage.AnimatedNavigationPage.GetLeaveAnimation(fromPage) :
                VCHelper.Controls.AnimatedNavigationPage.AnimatedNavigationPage.GetExitAnimation(fromPage);

            AnimateTransition(toController, enterAnimation, true, duration, transitionContext);
            AnimateTransition(fromController, exitAnimation, false, duration, transitionContext);
        }

        private void AnimateTransition(UIViewController controller, IAnimation transition, bool isIn, double duration, IUIViewControllerContextTransitioning transitionContext)
        {
            switch (transition)
            {
                case Fade _:
                    UIView.Animate(duration, () => controller.View.Alpha = isIn ? 1 : 0, () => CompleteTransition(transitionContext));
                    break;
                case Slide slide when slide.Direction != SlideDirection.None:
                    var size = UIScreen.MainScreen.Bounds.Size;
                    var center = controller.View.Center;
                    switch (slide.Direction)
                    {
                        case SlideDirection.FromLeftToRight:
                            {
                                if (isIn)
                                    controller.View.Center = new CGPoint(center.X - size.Width, center.Y);
                                var newCenter = new CGPoint(isIn ? center.X : center.X + size.Width, center.Y);
                                UIView.Animate(duration, () => controller.View.Center = newCenter, () => CompleteTransition(transitionContext));
                            }
                            break;
                        case SlideDirection.FromRightToLeft:
                            {
                                if (isIn)
                                    controller.View.Center = new CGPoint(center.X + size.Width, center.Y);
                                var newCenter = new CGPoint(isIn ? center.X : center.X - size.Width, center.Y);
                                UIView.Animate(duration, 0, UIViewAnimationOptions.CurveEaseOut, () => controller.View.Center = newCenter, () => CompleteTransition(transitionContext));
                            }
                            break;
                        case SlideDirection.FromTopToBottom:
                            {
                                if (isIn)
                                    controller.View.Center = new CGPoint(center.X, center.Y - size.Height);
                                var newCenter = new CGPoint(center.X, isIn ? center.Y : center.Y + size.Height);
                                UIView.Animate(duration, () => controller.View.Center = newCenter, () => CompleteTransition(transitionContext));
                            }
                            break;
                        case SlideDirection.FromBottomToTop:
                            {
                                if (isIn)
                                    controller.View.Center = new CGPoint(center.X, center.Y + size.Height);
                                var newCenter = new CGPoint(center.X, isIn ? center.Y : center.Y - size.Height);
                                UIView.Animate(duration, () => controller.View.Center = newCenter, () => CompleteTransition(transitionContext));
                            }
                            break;
                    }
                    break;
            }
        }

        private void CompleteTransition(IUIViewControllerContextTransitioning transitionContext)
        {
            transitionContext.FinishInteractiveTransition();
            transitionContext.CompleteTransition(!transitionContext.TransitionWasCancelled);
        }

        private Page GetPageFromController(UIViewController controller)
        {
            var renderer = controller.ChildViewControllers.LastOrDefault(x => x is IVisualElementRenderer) as IVisualElementRenderer;
            var page = renderer.Element as Page;
            return page;
        }

        public override double TransitionDuration(IUIViewControllerContextTransitioning transitionContext)
        {
            return 0.3;
        }
    }
}