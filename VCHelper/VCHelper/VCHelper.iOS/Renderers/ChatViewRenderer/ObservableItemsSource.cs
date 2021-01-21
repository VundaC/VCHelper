﻿using Foundation;
using System;
using System.Collections;
using System.Collections.Specialized;
using UIKit;
using Xamarin.Forms.Platform.iOS;

namespace VCHelper.iOS.Renderers.ChatViewRenderer
{
    public class ObservableItemsSource : IItemsViewSource
    {
        readonly UICollectionViewController _collectionViewController;
        readonly UICollectionView _collectionView;
        readonly bool _grouped;
        readonly int _section;
        readonly IEnumerable _itemsSource;
        bool _disposed;

        public ObservableItemsSource(IEnumerable itemSource, UICollectionViewController collectionViewController, int group = -1)
        {
            _collectionViewController = collectionViewController;
            _collectionView = _collectionViewController.CollectionView;

            _section = group < 0 ? 0 : group;
            _grouped = group >= 0;

            _itemsSource = itemSource;

            Count = ItemsCount();

            ((INotifyCollectionChanged)itemSource).CollectionChanged += CollectionChanged;
        }

        internal event NotifyCollectionChangedEventHandler CollectionItemsSourceChanged;

        public int Count { get; private set; }

        public int Section => _section;

        public object this[int index] => ElementAt(index);

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    ((INotifyCollectionChanged)_itemsSource).CollectionChanged -= CollectionChanged;
                }

                _disposed = true;
            }
        }

        public int ItemCountInGroup(nint group)
        {
            return Count;
        }

        public object Group(NSIndexPath indexPath)
        {
            return null;
        }

        public NSIndexPath GetIndexForItem(object item)
        {
            for (int n = 0; n < Count; n++)
            {
                if (this[n] == item)
                {
                    return NSIndexPath.Create(_section, n);
                }
            }

            return NSIndexPath.Create(-1, -1);
        }

        public int GroupCount => 1;

        public int ItemCount => Count;

        public object this[NSIndexPath indexPath]
        {
            get
            {
                if (indexPath.Section != _section)
                {
                    throw new ArgumentOutOfRangeException(nameof(indexPath));
                }

                return this[(int)indexPath.Item];
            }
        }

        void CollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            switch (args.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    Add(args);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    Remove(args);
                    break;
                case NotifyCollectionChangedAction.Replace:
                    Replace(args);
                    break;
                case NotifyCollectionChangedAction.Move:
                    Move(args);
                    break;
                case NotifyCollectionChangedAction.Reset:
                    Reload();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            CollectionItemsSourceChanged?.Invoke(this, args);
        }

        void Reload()
        {
            _collectionView.ReloadData();
            _collectionView.CollectionViewLayout.InvalidateLayout();
            Count = ItemsCount();
        }

        protected virtual NSIndexPath[] CreateIndexesFrom(int startIndex, int count)
        {
            var result = new NSIndexPath[count];

            for (int n = 0; n < count; n++)
            {
                result[n] = NSIndexPath.Create(_section, startIndex + n);
            }

            return result;
        }

        void Add(NotifyCollectionChangedEventArgs args)
        {
            if (ReloadRequired())
            {
                Reload();
                return;
            }

            var count = args.NewItems.Count;
            Count += count;
            var startIndex = args.NewStartingIndex > -1 ? args.NewStartingIndex : IndexOf(args.NewItems[0]);

            // Queue up the updates to the UICollectionView
            _collectionView.InsertItems(CreateIndexesFrom(startIndex, count));
        }

        void Remove(NotifyCollectionChangedEventArgs args)
        {
            var startIndex = args.OldStartingIndex;

            if (startIndex < 0)
            {
                // INCC implementation isn't giving us enough information to know where the removed items were in the
                // collection. So the best we can do is a ReloadData()
                Reload();
                return;
            }

            if (ReloadRequired())
            {
                Reload();
                return;
            }

            // If we have a start index, we can be more clever about removing the item(s) (and get the nifty animations)
            var count = args.OldItems.Count;
            Count -= count;

            // Queue up the updates to the UICollectionView
            _collectionView.DeleteItems(CreateIndexesFrom(startIndex, count));
        }

        void Replace(NotifyCollectionChangedEventArgs args)
        {
            var newCount = args.NewItems.Count;

            if (newCount == args.OldItems.Count)
            {
                var startIndex = args.NewStartingIndex > -1 ? args.NewStartingIndex : IndexOf(args.NewItems[0]);

                // We are replacing one set of items with a set of equal size; we can do a simple item range update
                _collectionView.ReloadItems(CreateIndexesFrom(startIndex, newCount));
                return;
            }

            // The original and replacement sets are of unequal size; this means that everything currently in view will 
            // have to be updated. So we just have to use ReloadData and let the UICollectionView update everything
            Reload();
        }

        void Move(NotifyCollectionChangedEventArgs args)
        {
            var count = args.NewItems.Count;

            if (count == 1)
            {
                // For a single item, we can use MoveItem and get the animation
                var oldPath = NSIndexPath.Create(_section, args.OldStartingIndex);
                var newPath = NSIndexPath.Create(_section, args.NewStartingIndex);

                UIView.PerformWithoutAnimation(() => _collectionView.MoveItem(oldPath, newPath));
                return;
            }

            var start = Math.Min(args.OldStartingIndex, args.NewStartingIndex);
            var end = Math.Max(args.OldStartingIndex, args.NewStartingIndex) + count;
            _collectionView.ReloadItems(CreateIndexesFrom(start, end));
        }

        internal int ItemsCount()
        {
            if (_itemsSource is IList list)
                return list.Count;

            int count = 0;
            foreach (var item in _itemsSource)
                count++;
            return count;
        }

        internal object ElementAt(int index)
        {
            if (_itemsSource is IList list)
                return list[index];

            int count = 0;
            foreach (var item in _itemsSource)
            {
                if (count == index)
                    return item;
                count++;
            }

            return -1;
        }

        internal int IndexOf(object item)
        {
            if (_itemsSource is IList list)
                return list.IndexOf(item);

            int count = 0;
            foreach (var i in _itemsSource)
            {
                if (i == item)
                    return count;
                count++;
            }

            return -1;
        }

        bool NotLoadedYet()
        {
            // If the UICollectionView hasn't actually been loaded, then calling InsertItems or DeleteItems is 
            // going to crash or get in an unusable state; instead, ReloadData should be used
            return !_collectionViewController.IsViewLoaded || _collectionViewController.View.Window == null;
        }

        bool ReloadRequired()
        {
            if (NotLoadedYet())
            {
                return true;
            }

            // UICollectionView doesn't like when we insert items into a completely empty un-grouped CV,
            // and it doesn't like when we insert items into a grouped CV with no actual cells (just empty groups)
            // In those circumstances, we just need to ask it to reload the data so it can get its internal
            // accounting in order

            if (!_grouped && _collectionView.NumberOfItemsInSection(_section) == 0)
            {
                return true;
            }

            return _collectionView.VisibleCells.Length == 0;
        }
    }
}