using Android.Content;
using AndroidX.Core.View;
using AndroidX.RecyclerView.Widget;
using System.Collections.Specialized;
using System.ComponentModel;
using VCHelper.Controls;
using VCHelper.Droid.Renderers.ChatViewRenderer;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using ARect = Android.Graphics.Rect;

[assembly: ExportRenderer(typeof(ChatView), typeof(ChatViewRenderer_Droid))]
namespace VCHelper.Droid.Renderers.ChatViewRenderer
{
    class ChatViewRenderer_Droid : ViewRenderer<ChatView, RecyclerView>
    {
        #region properties
        protected ChatAdapter ItemsViewAdapter;

        bool _disposed;

        EmptyViewAdapter _emptyViewAdapter;

        private ChatViewScrollListener _scrollListener;

        ScrollBarVisibility _defaultVerticalScrollVisibility = ScrollBarVisibility.Default;

        private INotifyCollectionChanged _collectionChangedListener;
        #endregion

        #region ctors
        public ChatViewRenderer_Droid(Context context) : base(context)
        {
            VerticalScrollBarEnabled = false;
            HorizontalScrollBarEnabled = false;
        }
        #endregion

        protected override void OnElementChanged(ElementChangedEventArgs<ChatView> e)
        {
            base.OnElementChanged(e);
            if (e.NewElement is ChatView newChat)
            {
                if (Control == null)
                    SetNativeControl(CreateNativeControl());

                UpdateItemsSource();

                UpdateLayoutManager();

                UpdateBackgroundColor();
                UpdateBackground();

                UpdateVerticalScrollBarVisibility();
                newChat.ScrollToRequested += ScrollToRequested;
            }
            if (e.OldElement is ChatView oldChat)
                oldChat.ScrollToRequested -= ScrollToRequested;
        }

        protected override void OnLayout(bool changed, int l, int t, int r, int b)
        {
            base.OnLayout(changed, l, t, r, b);
            ViewCompat.SetClipBounds(this, new ARect(0, 0, Width, Height));

            // After a direct (non-animated) scroll operation, we may need to make adjustments
            // to align the target item; if an adjustment is pending, execute it here.
            // (Deliberately checking the private member here rather than the property accessor; the accessor will
            // create a new ScrollHelper if needed, and there's no reason to do that until a Scroll is requested.)
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
                if (Element != null)
                {
                    TearDownOldElement(Element);

                    //if (Platform.GetRenderer(Element) == this)
                    //{
                    //    Element.ClearValue;
                    //}
                }
            }

            base.Dispose(disposing);
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs changedProperty)
        {
            base.OnElementPropertyChanged(sender, changedProperty);
            switch (changedProperty.PropertyName)
            {
                case nameof(ChatView.ItemsSource):
                    UpdateItemsSource();
                    break;
                case nameof(ChatView.ItemTemplate):
                    Control?.GetRecycledViewPool()?.Clear();
                    UpdateAdapter();
                    break;
                case nameof(ChatView.BackgroundColor):
                    UpdateBackgroundColor();
                    break;
                case nameof(ChatView.Background):
                    UpdateBackground();
                    break;
                case nameof(ChatView.VerticalScrollBarVisibility):
                    UpdateVerticalScrollBarVisibility();
                    break;
            }
        }

        protected virtual void UpdateItemsSource()
        {
            if (Element == null)
            {
                return;
            }
            SubscribeToItemsSource();
            UpdateAdapter();

            UpdateEmptyView();
        }

        private void SubscribeToItemsSource()
        {
            UnsubscribeFromItemsSource();
            _collectionChangedListener = Element?.ItemsSource as INotifyCollectionChanged;
            if (_collectionChangedListener != null)
                _collectionChangedListener.CollectionChanged += OnCollectionChanged;
        }

        private void UnsubscribeFromItemsSource()
        {
            if (_collectionChangedListener != null)
                _collectionChangedListener.CollectionChanged -= OnCollectionChanged;
            _collectionChangedListener = null;
        }

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add && e.NewStartingIndex == 0 && _scrollListener.VerticalOffset >= -50)
                ScrollTo(new ScrollToRequestEventArgs(0, 0, ScrollToPosition.Start, false));
        }

        protected virtual ChatAdapter CreateAdapter()
        {
            return new ChatAdapter(Element);
        }

        protected override RecyclerView CreateNativeControl()
        {
            return new RecyclerView(Context);
        }

        protected virtual void UpdateAdapter()
        {
            var oldItemViewAdapter = ItemsViewAdapter;

            ItemsViewAdapter = CreateAdapter();
            Control.SetAdapter(ItemsViewAdapter);

            if (_scrollListener != null)
            {
                Control.RemoveOnScrollListener(_scrollListener);
                _scrollListener.Dispose();
                _scrollListener = null;
            }
            _scrollListener = new ChatViewScrollListener(Element, ItemsViewAdapter);
            Control.AddOnScrollListener(_scrollListener);

            oldItemViewAdapter?.Dispose();
        }

        protected virtual void UpdateVerticalScrollBarVisibility()
        {
            if (_defaultVerticalScrollVisibility == ScrollBarVisibility.Default)
                _defaultVerticalScrollVisibility = VerticalScrollBarEnabled ? ScrollBarVisibility.Always : ScrollBarVisibility.Never;

            var newVerticalScrollVisibility = Element.VerticalScrollBarVisibility;

            if (newVerticalScrollVisibility == ScrollBarVisibility.Default)
                newVerticalScrollVisibility = _defaultVerticalScrollVisibility;

            Control.VerticalScrollBarEnabled = newVerticalScrollVisibility == ScrollBarVisibility.Always;
        }

        protected virtual void TearDownOldElement(Element oldElement)
        {
            if (oldElement == null)
            {
                return;
            }

            UnsubscribeFromItemsSource();

            // Stop listening for property changes
            oldElement.PropertyChanged -= OnElementPropertyChanged;

            if (ItemsViewAdapter != null)
            {
                // Unhook whichever adapter is active
                Control.SetAdapter(null);

                _emptyViewAdapter?.Dispose();
                ItemsViewAdapter?.Dispose();
            }
        }

        // TODO hartez 2018/08/09 09:30:17 Package up background color and flow direction providers so we don't have to re-implement them here	
        protected virtual void UpdateBackgroundColor(Color? color = null)
        {
            if (Element == null)
            {
                return;
            }

            Control.SetBackgroundColor((color ?? Element.BackgroundColor).ToAndroid());
        }

        protected virtual void UpdateBackground(Brush brush = null)
        {
            if (Element == null || Control == null)
                return;

            Brush background = Element.Background;

            Control.UpdateBackground(background);
        }

        protected virtual void UpdateEmptyView()
        {
            if (ItemsViewAdapter == null || Element == null)
            {
                return;
            }

            var emptyView = Element?.EmptyView;
            var emptyViewTemplate = Element?.EmptyViewTemplate;

            if (emptyView != null || emptyViewTemplate != null)
            {
                if (_emptyViewAdapter == null)
                {
                    _emptyViewAdapter = new EmptyViewAdapter(Element);
                }

                _emptyViewAdapter.EmptyView = emptyView;
                _emptyViewAdapter.EmptyViewTemplate = emptyViewTemplate;

                _emptyViewAdapter.NotifyDataSetChanged();
            }
            else
            {
            }
        }

        void ScrollToRequested(object sender, ScrollToRequestEventArgs args)
        {
            ScrollTo(args);
        }

        protected virtual void ScrollTo(ScrollToRequestEventArgs args)
        {
            if (Element == null || Control == null)
                return;

            var position = DetermineTargetPosition(args);
            if (position == -1)
                return;

            if (args.IsAnimated)
            {
                Control.SmoothScrollToPosition(position);
            }
            else
            {
                Control.ScrollToPosition(position);
            }
        }

        protected virtual void UpdateLayoutManager()
        {
            Control.SetLayoutManager(new LinearLayoutManager(Context)
            {
                ReverseLayout = true
            });
        }

        protected virtual int DetermineTargetPosition(ScrollToRequestEventArgs args)
        {
            if (args.Mode == ScrollToMode.Position)
            {
                // TODO hartez 2018/08/28 15:40:03 Need to handle group indices here as well	
                return args.Index;
            }

            return ItemsViewAdapter.GetPositionForItem(args.Item);
        }
    }
}