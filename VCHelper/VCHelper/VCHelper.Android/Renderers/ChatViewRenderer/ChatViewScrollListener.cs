using AndroidX.RecyclerView.Widget;
using VCHelper.Controls;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

namespace VCHelper.Droid.Renderers.ChatViewRenderer
{
    class ChatViewScrollListener : RecyclerView.OnScrollListener
    {
        protected ChatAdapter ItemsViewAdapter;
        bool _disposed;
        int _horizontalOffset;
        public int VerticalOffset { get; private set; }
        ChatView _itemsView;
        readonly bool _getCenteredItemOnXAndY = false;

        public ChatViewScrollListener(ChatView itemsView, ChatAdapter itemsViewAdapter) : this(itemsView, itemsViewAdapter, false)
        {
        }

        public ChatViewScrollListener(ChatView itemsView, ChatAdapter itemsViewAdapter, bool getCenteredItemOnXAndY)
        {
            _itemsView = itemsView;
            ItemsViewAdapter = itemsViewAdapter;
            _getCenteredItemOnXAndY = getCenteredItemOnXAndY;
        }

        public override void OnScrolled(RecyclerView recyclerView, int dx, int dy)
        {
            base.OnScrolled(recyclerView, dx, dy);

            // TODO: These offsets will be incorrect upon row size or count change.
            // They are currently provided in place of LayoutManager's default offset calculation
            // because it does not report accurate values in the presence of uneven rows.
            // See https://stackoverflow.com/questions/27507715/android-how-to-get-the-current-x-offset-of-recyclerview
            _horizontalOffset += dx;
            VerticalOffset += dy;
            var (First, Center, Last) = GetVisibleItemsIndex(recyclerView);

            var context = recyclerView.Context;
            var itemsViewScrolledEventArgs = new ItemsViewScrolledEventArgs
            {
                HorizontalDelta = context.FromPixels(dx),
                VerticalDelta = context.FromPixels(dy),
                HorizontalOffset = context.FromPixels(_horizontalOffset),
                VerticalOffset = context.FromPixels(VerticalOffset),
                FirstVisibleItemIndex = First,
                CenterItemIndex = Center,
                LastVisibleItemIndex = Last
            };

            _itemsView.SendScrolled(itemsViewScrolledEventArgs);

            // Don't send RemainingItemsThresholdReached event for non-linear layout managers
            // This can also happen if a layout pass has not happened yet
            if (Last != -1)
            {
                switch (_itemsView.RemainingItemsThreshold)
                {
                    case -1:
                        break;
                    case 0:
                        if (Last == ItemsViewAdapter.ItemCount - 1)
                            _itemsView.SendRemainingItemsThresholdReached();
                        break;
                    default:
                        if (ItemsViewAdapter.ItemCount - 1 - Last <= _itemsView.RemainingItemsThreshold)
                            _itemsView.SendRemainingItemsThresholdReached();
                        break;
                }
            }

            if (First != -1)
            {
                switch (_itemsView.RemainingStartItemsThreshold)
                {
                    case -1:
                        break;
                    default:
                        if (First < _itemsView.RemainingStartItemsThreshold - 1)
                            _itemsView.SendRemainingStartItemsThresholdReached();
                        break;
                }
            }
        }

        protected virtual (int First, int Center, int Last) GetVisibleItemsIndex(RecyclerView recyclerView)
        {
            var firstVisibleItemIndex = -1;
            var lastVisibleItemIndex = -1;
            var centerItemIndex = -1;

            if (recyclerView.GetLayoutManager() is LinearLayoutManager linearLayoutManager)
            {
                firstVisibleItemIndex = linearLayoutManager.FindFirstVisibleItemPosition();
                lastVisibleItemIndex = linearLayoutManager.FindLastVisibleItemPosition();
                centerItemIndex = recyclerView.CalculateCenterItemIndex(firstVisibleItemIndex, linearLayoutManager, _getCenteredItemOnXAndY);
            }
            return (firstVisibleItemIndex, centerItemIndex, lastVisibleItemIndex);
        }

        protected override void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                _itemsView = null;
                ItemsViewAdapter = null;
            }

            _disposed = true;

            base.Dispose(disposing);
        }
    }
}