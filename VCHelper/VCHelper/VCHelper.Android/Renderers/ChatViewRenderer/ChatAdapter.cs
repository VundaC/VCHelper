using Android.Content;
using Android.Views;
using AndroidX.RecyclerView.Widget;
using System;
using VCHelper.Controls;
using Xamarin.Forms.Platform.Android;

namespace VCHelper.Droid.Renderers.ChatViewRenderer
{
    public class ChatAdapter : RecyclerView.Adapter
    {
        protected readonly ChatView ItemsView;
        readonly Func<Xamarin.Forms.View, Context, ItemContentView> _createItemContentView;
        protected internal IItemsViewSource ItemsSource;

        bool _disposed;
        bool _usingItemTemplate = false;

        protected internal ChatAdapter(ChatView itemsView, Func<Xamarin.Forms.View, Context, ItemContentView> createItemContentView = null)
        {
            ItemsView = itemsView ?? throw new ArgumentNullException(nameof(itemsView));

            UpdateUsingItemTemplate();

            ItemsView.PropertyChanged += ItemsViewPropertyChanged;

            _createItemContentView = createItemContentView;
            ItemsSource = CreateItemsSource();

            if (_createItemContentView == null)
            {
                _createItemContentView = (view, context) => new ItemContentView(context);
            }
        }

        protected virtual IItemsViewSource CreateItemsSource()
        {
            return ItemsSourceFactory.Create(ItemsView, this);
        }

        protected virtual void ItemsViewPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs property)
        {
            switch (property.PropertyName)
            {
                case nameof(ChatView.ItemsSource):
                    UpdateItemsSource();
                    break;
                case nameof(ChatView.ItemTemplate):
                    UpdateUsingItemTemplate();
                    break;
            }
        }

        public override void OnViewRecycled(Java.Lang.Object holder)
        {
            if (holder is TemplatedItemViewHolder templatedItemViewHolder)
            {
                templatedItemViewHolder.Recycle(ItemsView);
            }

            base.OnViewRecycled(holder);
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            switch (holder)
            {
                case TemplatedItemViewHolder templatedItemViewHolder:
                    BindTemplatedItemViewHolder(templatedItemViewHolder, ItemsSource.GetItem(position));
                    break;
            }
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            var context = parent.Context;

            var itemContentView = _createItemContentView.Invoke(ItemsView, context);

            return new TemplatedItemViewHolder(itemContentView, ItemsView.ItemTemplate);
        }

        public override int ItemCount => ItemsSource.Count;

        public override int GetItemViewType(int position)
        {
            if (_usingItemTemplate)
            {
                return Xamarin.Forms.Platform.Android.ItemViewType.TemplatedItem;
            }

            // No template, just use the Text view
            return Xamarin.Forms.Platform.Android.ItemViewType.TextItem;
        }

        protected override void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    ItemsSource?.Dispose();
                    ItemsView.PropertyChanged -= ItemsViewPropertyChanged;
                }

                _disposed = true;

                base.Dispose(disposing);
            }
        }

        public virtual int GetPositionForItem(object item)
        {
            return ItemsSource.GetPosition(item);
        }

        protected virtual void BindTemplatedItemViewHolder(TemplatedItemViewHolder templatedItemViewHolder, object context)
        {
            templatedItemViewHolder.Bind(context, ItemsView);
        }

        void UpdateItemsSource()
        {
            ItemsSource?.Dispose();

            ItemsSource = CreateItemsSource();
        }

        void UpdateUsingItemTemplate()
        {
            _usingItemTemplate = ItemsView.ItemTemplate != null;
        }
    }
}