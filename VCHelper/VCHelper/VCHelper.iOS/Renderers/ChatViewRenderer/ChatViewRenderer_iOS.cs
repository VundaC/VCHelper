using Foundation;
using System.ComponentModel;
using UIKit;
using VCHelper.Controls;
using VCHelper.iOS.Renderers.ChatViewRenderer;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(ChatView), typeof(ChatViewRenderer_iOS))]
namespace VCHelper.iOS.Renderers.ChatViewRenderer
{
    public class ChatViewRenderer_iOS : ViewRenderer<ChatView, UIView>
    {
        ChatViewLayout _layout;
        bool _disposed;
        bool? _defaultVerticalScrollVisibility;

        public override UIViewController ViewController => Controller;

        protected ChatViewController Controller { get; private set; }

        [Xamarin.Forms.Internals.Preserve(Conditional = true)]
        public ChatViewRenderer_iOS()
        {
            AutoPackage = false;
        }

        public override SizeRequest GetDesiredSize(double widthConstraint, double heightConstraint)
        {
            return Control.GetSizeRequest(widthConstraint, heightConstraint, 0, 0);
        }

        protected override void OnElementChanged(ElementChangedEventArgs<ChatView> e)
        {
            TearDownOldElement(e.OldElement);
            SetUpNewElement(e.NewElement);

            base.OnElementChanged(e);
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs changedProperty)
        {
            base.OnElementPropertyChanged(sender, changedProperty);

            if (changedProperty.PropertyName == nameof(ChatView.ItemsSource))
            {
                UpdateItemsSource();
            }
            else if (changedProperty.PropertyName == nameof(ChatView.ItemTemplate))
            {
                UpdateLayout();
            }
            else if (changedProperty.PropertyName == nameof(ChatView.VerticalScrollBarVisibility))
            {
                UpdateVerticalScrollBarVisibility();
            }
        }

        protected ChatViewLayout SelectLayout()
        {
            return new ChatViewLayout();
        }

        protected virtual void TearDownOldElement(ChatView oldElement)
        {
            if (oldElement == null)
            {
                return;
            }

            // Stop listening for ScrollTo requests
            oldElement.ScrollToRequested -= ScrollToRequested;
        }

        protected virtual void SetUpNewElement(ChatView newElement)
        {
            if (newElement == null)
            {
                return;
            }

            UpdateLayout();
            Controller = CreateController(newElement, _layout);

            SetNativeControl(Controller.View);
            Controller.CollectionView.BackgroundColor = UIColor.Clear;
            UpdateVerticalScrollBarVisibility();
            // Listen for ScrollTo requests
            newElement.ScrollToRequested += ScrollToRequested;
        }

        protected virtual void UpdateLayout()
        {
            _layout = SelectLayout();

            if (Controller != null)
            {
                Controller.UpdateLayout(_layout);
            }
        }

        protected virtual void UpdateItemSizingStrategy()
        {
            UpdateLayout();
        }

        protected virtual void UpdateItemsSource()
        {
            Controller.UpdateItemsSource();
        }

        protected ChatViewController CreateController(ChatView newElement, ChatViewLayout layout)
        {
            return new ChatViewController(newElement, layout);
        }

        NSIndexPath DetermineIndex(ScrollToRequestEventArgs args)
        {
            if (args.Mode == ScrollToMode.Position)
            {
                if (args.GroupIndex == -1)
                {
                    return NSIndexPath.Create(0, args.Index);
                }

                return NSIndexPath.Create(args.GroupIndex, args.Index);
            }

            return Controller.GetIndexForItem(args.Item);
        }

        void UpdateVerticalScrollBarVisibility()
        {
            if (_defaultVerticalScrollVisibility == null)
                _defaultVerticalScrollVisibility = Controller.CollectionView.ShowsVerticalScrollIndicator;

            switch (Element.VerticalScrollBarVisibility)
            {
                case ScrollBarVisibility.Always:
                    Controller.CollectionView.ShowsVerticalScrollIndicator = true;
                    break;
                case ScrollBarVisibility.Never:
                    Controller.CollectionView.ShowsVerticalScrollIndicator = false;
                    break;
                case ScrollBarVisibility.Default:
                    Controller.CollectionView.ShowsVerticalScrollIndicator = _defaultVerticalScrollVisibility.Value;
                    break;
            }
        }

        protected virtual void ScrollToRequested(object sender, ScrollToRequestEventArgs args)
        {
            using (var indexPath = DetermineIndex(args))
            {
                if (!IsIndexPathValid(indexPath))
                {
                    // Specified path wasn't valid, or item wasn't found
                    return;
                }

                Controller.CollectionView.ScrollToItem(indexPath,
                    args.ScrollToPosition.ToCollectionViewScrollPosition(_layout.ScrollDirection), args.IsAnimated);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;

            if (disposing)
            {
                TearDownOldElement(Element);

                Controller?.Dispose();
                Controller = null;
            }

            base.Dispose(disposing);
        }

        protected bool IsIndexPathValid(NSIndexPath indexPath)
        {
            if (indexPath.Item < 0 || indexPath.Section < 0)
            {
                return false;
            }

            var collectionView = Controller.CollectionView;
            if (indexPath.Section >= collectionView.NumberOfSections())
            {
                return false;
            }

            if (indexPath.Item >= collectionView.NumberOfItemsInSection(indexPath.Section))
            {
                return false;
            }

            return true;
        }
    }
}