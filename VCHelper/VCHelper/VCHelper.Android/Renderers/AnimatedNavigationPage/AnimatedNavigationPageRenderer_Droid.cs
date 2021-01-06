using Android.Content;
using Android.OS;
using Android.Views;
#if __ANDROID_29__
using Fragment = AndroidX.Fragment.App.Fragment;
using FragmentTransaction = AndroidX.Fragment.App.FragmentTransaction;
#else
using Fragment = Android.Support.V4.App.Fragment;
using FragmentTransaction = Android.Support.V4.App.FragmentTransaction;
#endif
using System;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Xamarin.Forms.Platform.Android.AppCompat;
using VCHelper.Controls.AnimatedNavigationPage;
using VCHelper.Droid.Renderers.AnimatedNavigationPage;

[assembly: ExportRenderer(typeof(AnimatedNavigationPage), typeof(AnimatedNavigationPageRenderer_Droid))]
namespace VCHelper.Droid.Renderers.AnimatedNavigationPage
{
    public class AnimatedNavigationPageRenderer_Droid : NavigationPageRenderer
    {
        private bool IsLollipopOrNewer => Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop;

        public AnimatedNavigationPageRenderer_Droid(Context context) : base(context)
        {
        }

        protected override void SetupPageTransition(FragmentTransaction transaction, bool isPush)
        {
            if (!IsLollipopOrNewer)
            {
                base.SetupPageTransition(transaction, isPush);
                return;
            }
            var stackCount = Element.Navigation.NavigationStack.Count;
            var from = isPush ? Element.Navigation.NavigationStack[stackCount - 2] : Element.CurrentPage;
            var to = isPush ? Element.CurrentPage : Element.Navigation.NavigationStack[stackCount - 2];

            if (isPush)
            {
                var enterAnimation = VCHelper.Controls.AnimatedNavigationPage.AnimatedNavigationPage.GetEnterAnimation(to);
                var exitAnimation = VCHelper.Controls.AnimatedNavigationPage.AnimatedNavigationPage.GetLeaveAnimation(from);

                transaction.SetCustomAnimations(
                    GetNativeAnimation(enterAnimation, true),
                    GetNativeAnimation(exitAnimation, false));
            }
            else
            {
                var enterAnimation = VCHelper.Controls.AnimatedNavigationPage.AnimatedNavigationPage.GetReturnAnimation(to);
                var returnAnimation = VCHelper.Controls.AnimatedNavigationPage.AnimatedNavigationPage.GetReturnAnimation(to);
                var exitAnimation = VCHelper.Controls.AnimatedNavigationPage.AnimatedNavigationPage.GetExitAnimation(from);
                var leaveAnimation = VCHelper.Controls.AnimatedNavigationPage.AnimatedNavigationPage.GetExitAnimation(from);

                transaction.SetCustomAnimations(
                    GetNativeAnimation(enterAnimation, true),
                    GetNativeAnimation(leaveAnimation, false),
                    GetNativeAnimation(returnAnimation, true),
                    GetNativeAnimation(exitAnimation, false));
            }
        }

        protected virtual int GetNativeAnimation(IAnimation transition, bool isIn)
        {
            return transition switch
            {
                Fade _ => isIn ? Resource.Animation.fade_in : Resource.Animation.fade_out,
                Slide slide => GetSlideAnimation(slide.Direction, isIn),
                _ => 0
            };
        }

        private int GetSlideAnimation(SlideDirection direction, bool isIn)
        {
            switch (direction)
            {
                case SlideDirection.FromLeftToRight:
                    if (isIn)
                        return Resource.Animation.enter_left;
                    else
                        return Resource.Animation.exit_right;
                case SlideDirection.FromRightToLeft:
                    if (isIn)
                        return Resource.Animation.enter_right;
                    else
                        return Resource.Animation.exit_left;
                case SlideDirection.FromTopToBottom:
                    if (isIn)
                        return Resource.Animation.enter_top;
                    else
                        return Resource.Animation.exit_bottom;
                case SlideDirection.FromBottomToTop:
                    if (isIn)
                        return Resource.Animation.enter_bottom;
                    else
                        return Resource.Animation.exit_top;
                default:
                    return 0;
            }
        }
    }
}