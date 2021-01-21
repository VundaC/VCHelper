using Xamarin.Forms.Platform.Android;

namespace VCHelper.Droid.Renderers.ChatViewRenderer
{
    public interface ICollectionChangedNotifier
    {
        void NotifyDataSetChanged();
        void NotifyItemChanged(IItemsViewSource source, int startIndex);
        void NotifyItemInserted(IItemsViewSource source, int startIndex);
        void NotifyItemMoved(IItemsViewSource source, int fromPosition, int toPosition);
        void NotifyItemRangeChanged(IItemsViewSource source, int start, int end);
        void NotifyItemRangeInserted(IItemsViewSource source, int startIndex, int count);
        void NotifyItemRangeRemoved(IItemsViewSource source, int startIndex, int count);
        void NotifyItemRemoved(IItemsViewSource source, int startIndex);
    }
}