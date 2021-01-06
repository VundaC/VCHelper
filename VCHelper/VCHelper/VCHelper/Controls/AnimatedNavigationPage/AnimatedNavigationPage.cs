using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace VCHelper.Controls.AnimatedNavigationPage
{
    public class AnimatedNavigationPage : NavigationPage
    {
        public static readonly BindableProperty EnterAnimationProperty = BindableProperty.CreateAttached(
               "EnterAnimation",
               typeof(IAnimation),
               typeof(AnimatedNavigationPage),
               default,
               BindingMode.OneWay);

        public static readonly BindableProperty ExitAnimationProperty = BindableProperty.CreateAttached(
            "ExitAnimation",
            typeof(IAnimation),
            typeof(AnimatedNavigationPage),
            default,
            BindingMode.OneWay);

        public static readonly BindableProperty LeaveAnimationProperty = BindableProperty.CreateAttached(
            "LeaveAnimation",
            typeof(IAnimation),
            typeof(AnimatedNavigationPage),
            default,
            BindingMode.OneWay);

        public static readonly BindableProperty ReturnAnimationProperty = BindableProperty.CreateAttached(
            "ReturnAnimation",
            typeof(IAnimation),
            typeof(AnimatedNavigationPage),
            default,
            BindingMode.OneWay);

        public static IAnimation GetEnterAnimation(BindableObject bindable)
        {
            return (IAnimation)bindable.GetValue(EnterAnimationProperty);
        }

        public static void SetEnterAnimation(BindableObject bindable, IAnimation value)
        {
            bindable.SetValue(EnterAnimationProperty, value);
        }

        public static IAnimation GetExitAnimation(BindableObject bindable)
        {
            return (IAnimation)bindable.GetValue(ExitAnimationProperty);
        }

        public static void SetExitAnimation(BindableObject bindable, IAnimation value)
        {
            bindable.SetValue(ExitAnimationProperty, value);
        }

        public static IAnimation GetLeaveAnimation(BindableObject bindable)
        {
            return (IAnimation)bindable.GetValue(LeaveAnimationProperty);
        }

        public static void SetLeaveAnimation(BindableObject bindable, IAnimation value)
        {
            bindable.SetValue(LeaveAnimationProperty, value);
        }

        public static IAnimation GetReturnAnimation(BindableObject bindable)
        {
            return (IAnimation)bindable.GetValue(ReturnAnimationProperty);
        }

        public static void SetReturnAnimation(BindableObject bindable, IAnimation value)
        {
            bindable.SetValue(ReturnAnimationProperty, value);
        }

        public AnimatedNavigationPage()
        {
        }

        public AnimatedNavigationPage(Page root) : base(root)
        {
        }
    }
}
