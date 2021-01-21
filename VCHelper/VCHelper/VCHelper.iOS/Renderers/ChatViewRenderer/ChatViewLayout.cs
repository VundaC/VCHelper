using CoreGraphics;
using Foundation;
using System;
using System.Linq;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

namespace VCHelper.iOS.Renderers.ChatViewRenderer
{
    public class ChatViewLayout : UICollectionViewFlowLayout
    {
        bool _disposed;

        bool _adjustContentOffset;
        CGSize _adjustmentSize0;
        CGSize _adjustmentSize1;
        CGSize _currentSize;

        public nfloat ConstrainedDimension { get; set; }

        public Func<UICollectionViewCell> GetPrototype { get; set; }

        internal ItemSizingStrategy ItemSizingStrategy { get; private set; }

        public ChatViewLayout()
        {
            if (VersionsExtensions.IsiOS11OrNewer)
            {
                // `ContentInset` is actually the default value, but I'm leaving this here as a note to
                // future maintainers; it's likely that someone will want a Platform Specific to change this behavior
                // (Setting it to `SafeArea` lets you do the thing where the header/footer of your UICollectionView
                // fills the screen width in landscape while your items are automatically shifted to avoid the notch)
                SectionInsetReference = UICollectionViewFlowLayoutSectionInsetReference.ContentInset;
            }
        }

        public override bool FlipsHorizontallyInOppositeLayoutDirection => true;

        protected override void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;

            base.Dispose(disposing);
        }

        internal virtual void UpdateConstraints(CGSize size)
        {
            if (size == _currentSize)
            {
                return;
            }

            _currentSize = size;

            var newSize = new CGSize(Math.Floor(size.Width), Math.Floor(size.Height));
            ConstrainTo(newSize);

            UpdateCellConstraints();
        }

        internal void SetInitialConstraints(CGSize size)
        {
            _currentSize = size;
            ConstrainTo(size);
        }

        public void ConstrainTo(CGSize size)
        {
            ConstrainedDimension = size.Width;
            DetermineCellSize();
        }

        public virtual UIEdgeInsets GetInsetForSection(UICollectionView collectionView, UICollectionViewLayout layout,
            nint section)
        {
            return new UIEdgeInsets(0, 0, collectionView.NumberOfItemsInSection(section), 0);
        }

        public virtual nfloat GetMinimumInteritemSpacingForSection(UICollectionView collectionView,
            UICollectionViewLayout layout, nint section)
        {
            return (nfloat)0.0;
        }

        public virtual nfloat GetMinimumLineSpacingForSection(UICollectionView collectionView,
            UICollectionViewLayout layout, nint section)
        {
            return (nfloat)0.0;
        }

        public void PrepareCellForLayout(ItemsViewCell cell)
        {
            if (EstimatedItemSize == CGSize.Empty)
            {
                cell.ConstrainTo(ItemSize);
            }
            else
            {
                cell.ConstrainTo(ConstrainedDimension);
            }
        }

        public override bool ShouldInvalidateLayout(UICollectionViewLayoutAttributes preferredAttributes, UICollectionViewLayoutAttributes originalAttributes)
        {
            return preferredAttributes.Bounds != originalAttributes.Bounds || base.ShouldInvalidateLayout(preferredAttributes, originalAttributes);
        }

        protected void DetermineCellSize()
        {
            if (GetPrototype == null)
            {
                return;
            }

            // We set the EstimatedItemSize here for two reasons:
            // 1. If we don't set it, iOS versions below 10 will crash
            // 2. If GetPrototype() cannot return a cell because the items source is empty, we need to have
            //		an estimate set so that when a cell _does_ become available (i.e., when the items source
            //		has at least one item), Autolayout will kick in for the first cell and size it correctly
            // If GetPrototype() _can_ return a cell, this estimate will be updated once that cell is measured
            EstimatedItemSize = new CGSize(1, 1);

            ItemsViewCell prototype = null;

            if (CollectionView?.VisibleCells.Length > 0)
            {
                prototype = CollectionView.VisibleCells[0] as ItemsViewCell;
            }

            if (prototype == null)
            {
                prototype = GetPrototype() as ItemsViewCell;
            }

            if (prototype == null)
            {
                return;
            }

            // Constrain and measure the prototype cell
            prototype.ConstrainTo(ConstrainedDimension);
            var measure = prototype.Measure();

            // Autolayout is now enabled, and this is the size used to guess scrollbar size and progress
            EstimatedItemSize = measure;
        }

        protected void UpdateCellConstraints()
        {
            PrepareCellsForLayout(CollectionView.VisibleCells);
            PrepareCellsForLayout(CollectionView.GetVisibleSupplementaryViews(UICollectionElementKindSectionKey.Header));
            PrepareCellsForLayout(CollectionView.GetVisibleSupplementaryViews(UICollectionElementKindSectionKey.Footer));
        }

        void PrepareCellsForLayout(UICollectionReusableView[] cells)
        {
            for (int n = 0; n < cells.Length; n++)
            {
                if (cells[n] is ItemsViewCell constrainedCell)
                {
                    PrepareCellForLayout(constrainedCell);
                }
            }
        }

        public override UICollectionViewLayoutInvalidationContext GetInvalidationContext(UICollectionViewLayoutAttributes preferredAttributes, UICollectionViewLayoutAttributes originalAttributes)
        {
            if (preferredAttributes.RepresentedElementKind != UICollectionElementKindSectionKey.Header
                && preferredAttributes.RepresentedElementKind != UICollectionElementKindSectionKey.Footer)
            {
                return base.GetInvalidationContext(preferredAttributes, originalAttributes);
            }

            // Ensure that if this invalidation was triggered by header/footer changes, the header/footer are being invalidated

            UICollectionViewFlowLayoutInvalidationContext invalidationContext = new UICollectionViewFlowLayoutInvalidationContext();
            var indexPath = preferredAttributes.IndexPath;

            if (preferredAttributes.RepresentedElementKind == UICollectionElementKindSectionKey.Header)
            {
                invalidationContext.InvalidateSupplementaryElements(UICollectionElementKindSectionKey.Header, new[] { indexPath });
            }
            else if (preferredAttributes.RepresentedElementKind == UICollectionElementKindSectionKey.Footer)
            {
                invalidationContext.InvalidateSupplementaryElements(UICollectionElementKindSectionKey.Footer, new[] { indexPath });
            }

            return invalidationContext;
        }

        public override bool ShouldInvalidateLayoutForBoundsChange(CGRect newBounds)
        {
            if (newBounds.Size == _currentSize)
            {
                return base.ShouldInvalidateLayoutForBoundsChange(newBounds);
            }

            if (VersionsExtensions.IsiOS11OrNewer)
                UpdateConstraints(CollectionView.AdjustedContentInset.InsetRect(newBounds).Size);
            else
                UpdateConstraints(CollectionView.Bounds.Size);

            return true;
        }

        public override UICollectionViewLayoutAttributes[] LayoutAttributesForElementsInRect(CGRect rect)
        {
            var attrs = base.LayoutAttributesForElementsInRect(rect);
            //var newArray = new UICollectionViewLayoutAttributes[] { };
            if (CollectionView == null)
                return attrs;

            for (int i = 0; i < attrs.Count(); i++)
            {
                attrs[i].Transform = CGAffineTransform.Scale(attrs[i].Transform, 1, -1);
            }
            return attrs;
        }

        public override UICollectionViewLayoutAttributes LayoutAttributesForItem(NSIndexPath indexPath)
        {
            var attrs = base.LayoutAttributesForItem(indexPath);
            attrs.Transform = CGAffineTransform.Scale(attrs.Transform, 1, -1);
            return attrs;
        }

        public override void PrepareLayout()
        {
            base.PrepareLayout();

            TrackOffsetAdjustment();
        }

        public override CGPoint TargetContentOffsetForProposedContentOffset(CGPoint proposedContentOffset)
        {
            if (_adjustContentOffset)
            {
                _adjustContentOffset = false;

                if (proposedContentOffset.Y <= 50)
                    return base.TargetContentOffsetForProposedContentOffset(proposedContentOffset);
                // PrepareForCollectionViewUpdates detected that an item update was going to shift the viewport
                // and we want to make sure it stays in place
                return proposedContentOffset + ComputeOffsetAdjustment();
            }

            return base.TargetContentOffsetForProposedContentOffset(proposedContentOffset);
        }

        CGSize ComputeOffsetAdjustment()
        {
            var size = _adjustmentSize0 - CollectionViewContentSize;
            return new CGSize(Math.Abs(size.Width), Math.Abs(size.Height));
        }

        public override void PrepareForCollectionViewUpdates(UICollectionViewUpdateItem[] updateItems)
        {
            base.PrepareForCollectionViewUpdates(updateItems);

            // If this update will shift the visible items,  we'll have to adjust for 
            // that later in TargetContentOffsetForProposedContentOffset
            _adjustContentOffset = UpdateWillShiftVisibleItems(CollectionView, updateItems);
        }

        void TrackOffsetAdjustment()
        {
            // Keep track of the previous sizes of the CollectionView content so we can adjust the viewport
            // offsets if we're in ItemsUpdatingScrollMode.KeepItemsInView

            // We keep track of the last two adjustments because the only place we can consistently track this
            // is PrepareLayout, and by the time PrepareLayout has been called, the CollectionViewContentSize
            // has already been updated

            if (_adjustmentSize0.IsEmpty)
            {
                _adjustmentSize0 = CollectionViewContentSize;
            }
            else if (_adjustmentSize1.IsEmpty)
            {
                _adjustmentSize1 = CollectionViewContentSize;
            }
            else
            {
                _adjustmentSize0 = _adjustmentSize1;
                _adjustmentSize1 = CollectionViewContentSize;
            }
        }

        static bool UpdateWillShiftVisibleItems(UICollectionView collectionView, UICollectionViewUpdateItem[] updateItems)
        {
            // Find the first visible item
            var firstPath = collectionView.IndexPathsForVisibleItems.FindFirst();

            if (firstPath == null)
            {
                // No visible items to shift
                return false;
            }

            // Determine whether any of the new items will be "before" the first visible item
            foreach (var item in updateItems)
            {
                if (item.UpdateAction == UICollectionUpdateAction.Delete
                    || item.UpdateAction == UICollectionUpdateAction.Insert
                    || item.UpdateAction == UICollectionUpdateAction.Move)
                {
                    if (item.IndexPathAfterUpdate == null)
                    {
                        continue;
                    }

                    if (item.IndexPathAfterUpdate.IsLessThanOrEqualToPath(firstPath))
                    {
                        // If any of these items will end up "before" the first visible item, then the items will shift
                        return true;
                    }
                }
            }

            return false;
        }
    }
}