﻿using CoreGraphics;
using Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UIKit;
using VCHelper.Controls;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

namespace VCHelper.iOS.Renderers.ChatViewRenderer
{
    public class ChatViewController : UICollectionViewController
    {
        public const int EmptyTag = 333;

        public IItemsViewSource ItemsSource { get; protected set; }
        public ChatView ItemsView { get; }
        protected ChatViewLayout ItemsViewLayout { get; set; }
        bool _initialized;
        bool _isEmpty;
        bool _emptyViewDisplayed;
        bool _disposed;

        UIView _emptyUIView;
        VisualElement _emptyViewFormsElement;

        protected UICollectionViewDelegateFlowLayout Delegator { get; set; }

        public ChatViewController(ChatView itemsView, ChatViewLayout layout) : base(layout)
        {
            ItemsView = itemsView;
            ItemsViewLayout = layout;
            CollectionView.AllowsSelection = false;
            CollectionView.Bounces = false;
            CollectionView.Transform = CGAffineTransform.Scale(CollectionView.Transform, 1, -1);
            CollectionView.ScrollIndicatorInsets = new UIEdgeInsets(0, 0, 0, CollectionView.Frame.Width - 8);
        }

        public void UpdateLayout(ChatViewLayout newLayout)
        {
            // Ignore calls to this method if the new layout is the same as the old one
            if (CollectionView.CollectionViewLayout == newLayout)
                return;

            ItemsViewLayout = newLayout;

            _initialized = false;

            EnsureLayoutInitialized();

            if (_initialized)
            {
                // Reload the data so the currently visible cells get laid out according to the new layout
                CollectionView.ReloadData();
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            _disposed = true;

            if (disposing)
            {
                ItemsSource?.Dispose();

                CollectionView.Delegate = null;
                Delegator?.Dispose();

                _emptyUIView?.Dispose();
                _emptyUIView = null;

                _emptyViewFormsElement = null;

                ItemsViewLayout?.Dispose();
                CollectionView?.Dispose();
            }

            base.Dispose(disposing);
        }

        public override UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath)
        {
            var cell = collectionView.DequeueReusableCell(DetermineCellReuseId(), indexPath) as UICollectionViewCell;

            switch (cell)
            {
                case DefaultCell defaultCell:
                    UpdateDefaultCell(defaultCell, indexPath);
                    break;
                case TemplatedCell templatedCell:
                    UpdateTemplatedCell(templatedCell, indexPath);
                    break;
            }

            return cell;
        }

        public override nint GetItemsCount(UICollectionView collectionView, nint section)
        {
            if (!_initialized)
            {
                return 0;
            }

            CheckForEmptySource();

            return ItemsSource.ItemCountInGroup(section);
        }

        void CheckForEmptySource()
        {
            var wasEmpty = _isEmpty;

            _isEmpty = ItemsSource.ItemCount == 0;

            if (wasEmpty != _isEmpty)
            {
                UpdateEmptyViewVisibility(_isEmpty);
            }

            if (wasEmpty && !_isEmpty)
            {
                // If we're going from empty to having stuff, it's possible that we've never actually measured
                // a prototype cell and our itemSize or estimatedItemSize are wrong/unset
                // So trigger a constraint update; if we need a measurement, that will make it happen
                ItemsViewLayout.ConstrainTo(CollectionView.Bounds.Size);
            }
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            ItemsSource = CreateItemsViewSource();

            if (!VersionsExtensions.IsiOS11OrNewer)
                AutomaticallyAdjustsScrollViewInsets = false;
            else
            {
                // We set this property to keep iOS from trying to be helpful about insetting all the 
                // CollectionView content when we're in landscape mode (to avoid the notch)
                // The SetUseSafeArea Platform Specific is already taking care of this for us 
                // That said, at some point it's possible folks will want a PS for controlling this behavior
                CollectionView.ContentInsetAdjustmentBehavior = UIScrollViewContentInsetAdjustmentBehavior.Never;
            }

            RegisterViewTypes();
        }

        public override void ViewWillLayoutSubviews()
        {
            base.ViewWillLayoutSubviews();

            if (!_initialized)
            {
                UpdateEmptyView();
            }

            // We can't set this up during ViewDidLoad, because Forms does other stuff that resizes the view
            // and we end up with massive layout errors. And View[Will/Did]Appear do not fire for this controller
            // reliably. So until one of those options is cleared up, we set this flag so that the initial constraints
            // are set up the first time this method is called.
            EnsureLayoutInitialized();

            if (_initialized)
            {
                LayoutEmptyView();
            }
        }

        void EnsureLayoutInitialized()
        {
            if (_initialized)
            {
                return;
            }

            if (!ItemsView.IsVisible)
            {
                // If the CollectionView starts out invisible, we'll get a layout pass with a size of 1,1 and everything will
                // go pear-shaped. So until the first time this CollectionView is visible, we do nothing.
                return;
            }

            _initialized = true;

            ItemsViewLayout.GetPrototype = GetPrototype;

            Delegator = CreateDelegator();
            CollectionView.Delegate = Delegator;

            ItemsViewLayout.SetInitialConstraints(CollectionView.Bounds.Size);
            CollectionView.SetCollectionViewLayout(ItemsViewLayout, false);
        }

        protected virtual UICollectionViewDelegateFlowLayout CreateDelegator()
        {
            return new ChatDelegator(ItemsViewLayout, this);
        }

        protected virtual IItemsViewSource CreateItemsViewSource()
        {
            return ItemsSourceFactory.Create(ItemsView.ItemsSource, this);
        }

        public virtual void UpdateItemsSource()
        {
            ItemsSource = CreateItemsViewSource();
            CollectionView.ReloadData();
            CollectionView.CollectionViewLayout.InvalidateLayout();
        }

        public override nint NumberOfSections(UICollectionView collectionView)
        {
            if (!_initialized)
            {
                return 0;
            }

            CheckForEmptySource();
            return ItemsSource.GroupCount;
        }

        protected virtual void UpdateDefaultCell(DefaultCell cell, NSIndexPath indexPath)
        {
            cell.Label.Text = ItemsSource[indexPath].ToString();

            if (cell is ItemsViewCell constrainedCell)
            {
                ItemsViewLayout.PrepareCellForLayout(constrainedCell);
            }
        }

        protected virtual void UpdateTemplatedCell(TemplatedCell cell, NSIndexPath indexPath)
        {
            cell.ContentSizeChanged -= CellContentSizeChanged;

            cell.Bind(ItemsView.ItemTemplate, ItemsSource[indexPath], ItemsView);

            cell.ContentSizeChanged += CellContentSizeChanged;

            ItemsViewLayout.PrepareCellForLayout(cell);
        }

        public virtual NSIndexPath GetIndexForItem(object item)
        {
            return ItemsSource.GetIndexForItem(item);
        }

        protected object GetItemAtIndex(NSIndexPath index)
        {
            return ItemsSource[index];
        }

        void CellContentSizeChanged(object sender, EventArgs e)
        {
            if (_disposed)
                return;

            Layout?.InvalidateLayout();
        }

        protected virtual string DetermineCellReuseId()
        {
            if (ItemsView.ItemTemplate != null)
            {
                return VerticalCell.ReuseId;
            }

            return VerticalDefaultCell.ReuseId;
        }

        UICollectionViewCell GetPrototype()
        {
            if (ItemsSource.ItemCount == 0)
            {
                return null;
            }

            var group = 0;

            if (ItemsSource.GroupCount > 1)
            {
                // If we're in a grouping situation, then we need to make sure we find an actual data item
                // to use for our prototype cell. It's possible that we have empty groups.
                for (int n = 0; n < ItemsSource.GroupCount; n++)
                {
                    if (ItemsSource.ItemCountInGroup(n) > 0)
                    {
                        group = n;
                        break;
                    }
                }
            }

            var indexPath = NSIndexPath.Create(group, 0);

            return CreateMeasurementCell(indexPath);
        }

        protected virtual void RegisterViewTypes()
        {
            CollectionView.RegisterClassForCell(typeof(VerticalDefaultCell), VerticalDefaultCell.ReuseId);
            CollectionView.RegisterClassForCell(typeof(VerticalCell), VerticalCell.ReuseId);
        }

        internal void UpdateEmptyView()
        {
            UpdateView(ItemsView?.EmptyView, ItemsView?.EmptyViewTemplate, ref _emptyUIView, ref _emptyViewFormsElement);

            // If the empty view is being displayed, we might need to update it
            UpdateEmptyViewVisibility(ItemsSource?.ItemCount == 0);
        }

        protected virtual CGRect DetermineEmptyViewFrame()
        {
            return new CGRect(CollectionView.Frame.X, CollectionView.Frame.Y,
                CollectionView.Frame.Width, CollectionView.Frame.Height);
        }

        void LayoutEmptyView()
        {
            if (_emptyUIView == null)
            {
                UpdateEmptyView();
                return;
            }

            var frame = DetermineEmptyViewFrame();

            _emptyUIView.Frame = frame;

            if (_emptyViewFormsElement != null && ItemsView.LogicalChildren.Contains(_emptyViewFormsElement))
                _emptyViewFormsElement.Layout(frame.ToRectangle());
        }

        protected void RemeasureLayout(VisualElement formsElement)
        {
            var request = formsElement.Measure(CollectionView.Frame.Width, double.PositiveInfinity, MeasureFlags.IncludeMargins);
            Xamarin.Forms.Layout.LayoutChildIntoBoundingRegion(formsElement, new Rectangle(0, 0, CollectionView.Frame.Width, request.Request.Height));
        }

        protected void OnFormsElementMeasureInvalidated(object sender, EventArgs e)
        {
            if (sender is VisualElement formsElement)
            {
                HandleFormsElementMeasureInvalidated(formsElement);
            }
        }

        protected virtual void HandleFormsElementMeasureInvalidated(VisualElement formsElement)
        {
            RemeasureLayout(formsElement);
        }

        internal void UpdateView(object view, DataTemplate viewTemplate, ref UIView uiView, ref VisualElement formsElement)
        {
            // Is view set on the ItemsView?
            if (view == null)
            {
                if (formsElement != null)
                    Platform.GetRenderer(formsElement)?.DisposeRendererAndChildren();

                uiView?.Dispose();
                uiView = null;

                formsElement = null;
            }
            else
            {
                // Create the native renderer for the view, and keep the actual Forms element (if any)
                // around for updating the layout later
                (uiView, formsElement) = TemplateHelpers.RealizeView(view, viewTemplate, ItemsView);
            }
        }

        void UpdateEmptyViewVisibility(bool isEmpty)
        {
            if (isEmpty && _emptyUIView != null)
            {
                var emptyView = CollectionView.ViewWithTag(EmptyTag);

                if (emptyView != null)
                {
                    emptyView.RemoveFromSuperview();
                    ItemsView.RemoveLogicalChild(_emptyViewFormsElement);
                }

                _emptyUIView.Tag = EmptyTag;

                var collectionViewContainer = CollectionView.Superview;
                collectionViewContainer.AddSubview(_emptyUIView);

                LayoutEmptyView();

                if (_emptyViewFormsElement != null)
                {
                    if (ItemsView.EmptyViewTemplate == null)
                    {
                        ItemsView.AddLogicalChild(_emptyViewFormsElement);
                    }

                    // Now that the native empty view's frame is sized to the UICollectionView, we need to handle
                    // the Forms layout for its content
                    _emptyViewFormsElement.Layout(_emptyUIView.Frame.ToRectangle());
                }

                _emptyViewDisplayed = true;
            }
            else
            {
                // Is the empty view currently in the background? Swap back to the default.
                if (_emptyViewDisplayed)
                {
                    _emptyUIView.RemoveFromSuperview();
                    _emptyUIView.Dispose();
                    _emptyUIView = null;

                    ItemsView.RemoveLogicalChild(_emptyViewFormsElement);
                }

                _emptyViewDisplayed = false;
            }
        }

        TemplatedCell CreateAppropriateCellForLayout()
        {
            var frame = new CGRect(0, 0, ItemsViewLayout.EstimatedItemSize.Width, ItemsViewLayout.EstimatedItemSize.Height);

            return new VerticalCell(frame);
        }

        public TemplatedCell CreateMeasurementCell(NSIndexPath indexPath)
        {
            if (ItemsView.ItemTemplate == null)
            {
                return null;
            }

            TemplatedCell templatedCell = CreateAppropriateCellForLayout();

            UpdateTemplatedCell(templatedCell, indexPath);

            return templatedCell;
        }

        public UIEdgeInsets GetInsetForSection(UICollectionView collectionView, nint section)
        {
            var uIEdgeInsets = ItemsViewLayout.GetInsetForSection(collectionView, ItemsViewLayout, section);

            // If we're grouping, we'll need to inset the sections to maintain the item spacing between the 
            // groups and/or their group headers/footers

            nfloat lineSpacing = ItemsViewLayout.GetMinimumLineSpacingForSection(collectionView, ItemsViewLayout, section);

            return new UIEdgeInsets(lineSpacing + uIEdgeInsets.Top, uIEdgeInsets.Left,
                uIEdgeInsets.Bottom, uIEdgeInsets.Right);
        }
    }
}