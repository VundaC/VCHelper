using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UIKit;
using Xamarin.Forms.Platform.iOS;

namespace VCHelper.iOS.Renderers.ChatViewRenderer
{
    public class ItemsSourceFactory
    {
        public static IItemsViewSource Create(IEnumerable itemsSource, UICollectionViewController collectionViewController)
        {
            if (itemsSource == null)
            {
                return new EmptySource();
            }

            switch (itemsSource)
            {
                case IList list when itemsSource is INotifyCollectionChanged:
                    return new ObservableItemsSource(list, collectionViewController);
                case IEnumerable enumerable when itemsSource is INotifyCollectionChanged:
                    return new ObservableItemsSource(enumerable, collectionViewController);
                case IEnumerable<object> generic:
                    return new ListSource(generic);
            }

            return new ListSource(itemsSource);
        }
    }
}