using CoreGraphics;
using Foundation;
using Xamarin.Forms;

namespace VCHelper.iOS.Renderers.ChatViewRenderer
{
    public class VerticalCell : WidthConstrainedTemplatedCell
    {
        public static NSString ReuseId = new NSString("Xamarin.Forms.Platform.iOS.VerticalCell");

        [Export("initWithFrame:")]
        [Xamarin.Forms.Internals.Preserve(Conditional = true)]
        public VerticalCell(CGRect frame) : base(frame)
        {
        }

        public override CGSize Measure()
        {
            var measure = VisualElementRenderer.Element.Measure(ConstrainedDimension,
                double.PositiveInfinity, MeasureFlags.IncludeMargins);

            return new CGSize(ConstrainedDimension, measure.Request.Height);
        }
    }
}