﻿using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VCHelper.Droid.Extensions;
using Xamarin.Forms.Platform.Android;

namespace VCHelper.Droid.Renderers.ChatViewRenderer
{
    public class ChatAdapterNotifier : ICollectionChangedNotifier
    {
        readonly RecyclerView.Adapter _adapter;

        public ChatAdapterNotifier(RecyclerView.Adapter adapter)
        {
            _adapter = adapter;
        }

        public void NotifyDataSetChanged()
        {
            if (IsValidAdapter())
                _adapter.NotifyDataSetChanged();
        }

        public void NotifyItemChanged(IItemsViewSource source, int startIndex)
        {
            if (IsValidAdapter())
                _adapter.NotifyItemChanged(startIndex);
        }

        public void NotifyItemInserted(IItemsViewSource source, int startIndex)
        {
            if (IsValidAdapter())
                _adapter.NotifyItemInserted(startIndex);
        }

        public void NotifyItemMoved(IItemsViewSource source, int fromPosition, int toPosition)
        {
            if (IsValidAdapter())
                _adapter.NotifyItemMoved(fromPosition, toPosition);
        }

        public void NotifyItemRangeChanged(IItemsViewSource source, int start, int end)
        {
            if (IsValidAdapter())
                _adapter.NotifyItemRangeChanged(start, end);
        }

        public void NotifyItemRangeInserted(IItemsViewSource source, int startIndex, int count)
        {
            if (IsValidAdapter())
                _adapter.NotifyItemRangeInserted(startIndex, count);
        }

        public void NotifyItemRangeRemoved(IItemsViewSource source, int startIndex, int count)
        {
            if (IsValidAdapter())
                _adapter.NotifyItemRangeRemoved(startIndex, count);
        }

        public void NotifyItemRemoved(IItemsViewSource source, int startIndex)
        {
            if (IsValidAdapter())
                _adapter.NotifyItemRemoved(startIndex);
        }

        internal bool IsValidAdapter()
        {
            if (_adapter == null || _adapter.IsDisposed())
                return false;

            return true;
        }
    }
}