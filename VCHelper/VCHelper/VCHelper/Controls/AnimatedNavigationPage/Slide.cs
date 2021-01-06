using Xamarin.Forms;

namespace VCHelper.Controls.AnimatedNavigationPage
{
    public class Slide : Animation
    {
        public static readonly BindableProperty DirectionProperty = BindableProperty.Create(
            nameof(Direction),
            typeof(SlideDirection),
            typeof(Slide),
            SlideDirection.None,
            BindingMode.OneWay);

        public SlideDirection Direction
        {
            get => (SlideDirection)GetValue(DirectionProperty);
            set => SetValue(DirectionProperty, value);
        }
    }

    public enum SlideDirection
    {
        FromLeftToRight,
        FromRightToLeft,
        FromTopToBottom,
        FromBottomToTop,
        None
    }
}
