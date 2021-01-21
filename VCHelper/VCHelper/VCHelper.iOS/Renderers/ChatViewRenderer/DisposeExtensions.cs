using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

namespace VCHelper.iOS.Renderers.ChatViewRenderer
{
    static class DisposeHelpers
    {
        internal static void DisposeModalAndChildRenderers(this Element view)
        {
            IVisualElementRenderer renderer;
            var rendererProperty = GetRendererProperty();
            foreach (Element child in view.Descendants())
            {
                if (child is VisualElement ve)
                {
                    renderer = Platform.GetRenderer(ve);
                    child.ClearValue(rendererProperty);

                    if (renderer != null)
                    {
                        renderer.NativeView.RemoveFromSuperview();
                        renderer.Dispose();
                    }
                }
            }

            if (view is VisualElement visualElement)
            {
                renderer = Platform.GetRenderer(visualElement);
                if (renderer != null)
                {
#if __MOBILE__
                    if (renderer.ViewController != null)
                    {
                        if (renderer.ViewController.ParentViewController is IUIAdaptivePresentationControllerDelegate modalWrapper)
                            modalWrapper.Dispose();
                    }
#endif
                    renderer.NativeView.RemoveFromSuperview();
                    renderer.Dispose();
                }
                view.ClearValue(rendererProperty);
            }
        }

        internal static void DisposeRendererAndChildren(this IVisualElementRenderer rendererToRemove)
        {
            if (rendererToRemove == null)
                return;
            var rendererProperty = GetRendererProperty();
            if (rendererToRemove.Element != null && Platform.GetRenderer(rendererToRemove.Element) == rendererToRemove)
                rendererToRemove.Element.ClearValue(rendererProperty);

            if (rendererToRemove.NativeView != null)
            {
                var subviews = rendererToRemove.NativeView.Subviews;
                for (var i = 0; i < subviews.Length; i++)
                {
                    if (subviews[i] is IVisualElementRenderer childRenderer)
                        DisposeRendererAndChildren(childRenderer);
                }
                rendererToRemove.NativeView.RemoveFromSuperview();
            }
            rendererToRemove.Dispose();
        }

        static BindableProperty GetRendererProperty()
        {
            return typeof(Platform).GetProperty("RendererProperty", System.Reflection.BindingFlags.Static).GetValue(null, null) as BindableProperty;
        }
    }
}