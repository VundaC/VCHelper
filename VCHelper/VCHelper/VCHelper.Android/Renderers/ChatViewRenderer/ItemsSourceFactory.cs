using AndroidX.RecyclerView.Widget;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

namespace VCHelper.Droid.Renderers.ChatViewRenderer
{
    public class ItemsSourceFactory
    {
        public static IItemsViewSource Create(IEnumerable itemsSource, ICollectionChangedNotifier notifier)
        {
            if (itemsSource == null)
            {
                return new EmptySource();
            }

            switch (itemsSource)
            {
                case IList list when itemsSource is INotifyCollectionChanged:
                    return new ObservableItemsSource(new MarshalingObservableCollection(list), notifier);
                case IEnumerable _ when itemsSource is INotifyCollectionChanged:
                    return new ObservableItemsSource(itemsSource as IEnumerable, notifier);
                case IEnumerable<object> generic:
                    return new ListSource(generic);
            }

            return new ListSource(itemsSource);
        }

        public static IItemsViewSource Create(IEnumerable itemsSource, RecyclerView.Adapter adapter)
        {
            return Create(itemsSource, new ChatAdapterNotifier(adapter));
        }

        public static IItemsViewSource Create(ItemsView itemsView, RecyclerView.Adapter adapter)
        {
            return Create(itemsView.ItemsSource, adapter);
        }
    }
}