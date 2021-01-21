﻿using CoreGraphics;
using Foundation;
using System;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Platform.iOS;

namespace VCHelper.iOS.Renderers.ChatViewRenderer
{
    public abstract class TemplatedCell : ItemsViewCell
    {
        public event EventHandler<EventArgs> ContentSizeChanged;

        protected CGSize ConstrainedSize;

        protected nfloat ConstrainedDimension;

        DataTemplate _currentTemplate;

        // Keep track of the cell size so we can verify whether a measure invalidation 
        // actually changed the size of the cell
        Size _size;

        [Export("initWithFrame:")]
        [Xamarin.Forms.Internals.Preserve(Conditional = true)]
        protected TemplatedCell(CGRect frame) : base(frame)
        {
        }

        internal IVisualElementRenderer VisualElementRenderer { get; private set; }

        public override void ConstrainTo(CGSize constraint)
        {
            ClearConstraints();
            ConstrainedSize = constraint;
        }

        public override void ConstrainTo(nfloat constant)
        {
            ClearConstraints();
            ConstrainedDimension = constant;
        }

        protected void ClearConstraints()
        {
            ConstrainedSize = default;
            ConstrainedDimension = default;
        }

        public override UICollectionViewLayoutAttributes PreferredLayoutAttributesFittingAttributes(
            UICollectionViewLayoutAttributes layoutAttributes)
        {
            var preferredAttributes = base.PreferredLayoutAttributesFittingAttributes(layoutAttributes);

            // Measure this cell (including the Forms element) if there is no constrained size
            var size = ConstrainedSize == default(CGSize) ? Measure() : ConstrainedSize;

            // Update the size of the root view to accommodate the Forms element
            var nativeView = VisualElementRenderer.NativeView;
            nativeView.Frame = new CGRect(CGPoint.Empty, size);

            // Layout the Forms element 
            var nativeBounds = nativeView.Frame.ToRectangle();
            VisualElementRenderer.Element.Layout(nativeBounds);
            _size = nativeBounds.Size;

            // Adjust the preferred attributes to include space for the Forms element
            preferredAttributes.Frame = new CGRect(preferredAttributes.Frame.Location, size);

            return preferredAttributes;
        }

        public void Bind(DataTemplate template, object bindingContext, ItemsView itemsView)
        {
            var oldElement = VisualElementRenderer?.Element;

            // Run this through the extension method in case it's really a DataTemplateSelector
            var itemTemplate = template.SelectDataTemplate(bindingContext, itemsView);

            if (itemTemplate != _currentTemplate)
            {
                // Remove the old view, if it exists
                if (oldElement != null)
                {
                    oldElement.MeasureInvalidated -= MeasureInvalidated;
                    itemsView.RemoveLogicalChild(oldElement);
                    ClearSubviews();
                    _size = Size.Zero;
                }

                // Create the content and renderer for the view 
                var view = itemTemplate.CreateContent() as View;

                // Set the binding context _before_ we create the renderer; that way, it's available during OnElementChanged
                view.BindingContext = bindingContext;

                var renderer = TemplateHelpers.CreateRenderer(view);
                SetRenderer(renderer);

                // And make the new Element a "child" of the ItemsView
                // We deliberately do this _after_ setting the binding context for the new element;
                // if we do it before, the element briefly inherits the ItemsView's bindingcontext and we 
                // emit a bunch of needless binding errors
                itemsView.AddLogicalChild(view);

                // Prevents the use of default color when there are VisualStateManager with Selected state setting the background color
                // First we check whether the cell has the default selected background color; if it does, then we should check
                // to see if the cell content is the VSM to set a selected color 
                var gray = VersionsExtensions.IsiOS13OrNewer ? UIColor.SystemGrayColor : UIColor.Gray;
                if (SelectedBackgroundView.BackgroundColor == gray && IsUsingVSMForSelectionColor(view))
                {
                    SelectedBackgroundView = new UIView
                    {
                        BackgroundColor = UIColor.Clear
                    };
                }
            }
            else
            {
                // Same template, different data
                var currentElement = VisualElementRenderer?.Element;

                if (currentElement != null)
                    currentElement.BindingContext = bindingContext;
            }

            _currentTemplate = itemTemplate;
        }

        void SetRenderer(IVisualElementRenderer renderer)
        {
            VisualElementRenderer = renderer;
            var nativeView = VisualElementRenderer.NativeView;

            InitializeContentConstraints(nativeView);

            renderer.Element.MeasureInvalidated += MeasureInvalidated;
        }

        protected void Layout(CGSize constraints)
        {
            var nativeView = VisualElementRenderer.NativeView;

            var width = constraints.Width;
            var height = constraints.Height;

            VisualElementRenderer.Element.Measure(width, height, MeasureFlags.IncludeMargins);

            nativeView.Frame = new CGRect(0, 0, width, height);

            VisualElementRenderer.Element.Layout(nativeView.Frame.ToRectangle());
        }

        void ClearSubviews()
        {
            for (int n = ContentView.Subviews.Length - 1; n >= 0; n--)
            {
                ContentView.Subviews[n].RemoveFromSuperview();
            }
        }

        bool IsUsingVSMForSelectionColor(View view)
        {
            var groups = VisualStateManager.GetVisualStateGroups(view);
            for (var groupIndex = 0; groupIndex < groups.Count; groupIndex++)
            {
                var group = groups[groupIndex];
                for (var stateIndex = 0; stateIndex < group.States.Count; stateIndex++)
                {
                    var state = group.States[stateIndex];
                    if (state.Name != VisualStateManager.CommonStates.Selected)
                    {
                        continue;
                    }

                    for (var setterIndex = 0; setterIndex < state.Setters.Count; setterIndex++)
                    {
                        var setter = state.Setters[setterIndex];
                        if (setter.Property.PropertyName == VisualElement.BackgroundColorProperty.PropertyName)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        public override bool Selected
        {
            get => base.Selected;
            set
            {
                base.Selected = value;

                var element = VisualElementRenderer?.Element;

                if (element != null)
                {
                    VisualStateManager.GoToState(element, value
                        ? VisualStateManager.CommonStates.Selected
                        : VisualStateManager.CommonStates.Normal);
                }
            }
        }

        protected abstract (bool, Size) NeedsContentSizeUpdate(Size currentSize);

        void MeasureInvalidated(object sender, EventArgs args)
        {
            var (needsUpdate, toSize) = NeedsContentSizeUpdate(_size);

            if (!needsUpdate)
            {
                return;
            }

            // Cache the size for next time
            _size = toSize;

            // Let the controller know that things need to be laid out again
            OnContentSizeChanged();
        }

        protected void OnContentSizeChanged()
        {
            ContentSizeChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}