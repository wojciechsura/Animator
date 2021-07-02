using Animator.Editor.BusinessLogic.ViewModels.Document;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Animator.Editor.Controls
{
    public class DocumentTabHeader : Panel
    {
        private class ChildSizeInfos
        {
            public ChildSizeInfos(List<UIElement> pinnedChildren, List<UIElement> regularChildren)
            {
                this.PinnedChildren = pinnedChildren;
                this.RegularChildren = regularChildren;
            }

            public readonly List<UIElement> PinnedChildren;
            public readonly List<UIElement> RegularChildren;
        }

        private ChildSizeInfos GenerateSizeInfo(bool measureChildren)
        {
            List<UIElement> pinnedChildren = new List<UIElement>();
            List<UIElement> regularChildren = new List<UIElement>();

            for (int i = 0; i < InternalChildren.Count; i++)
            {
                if (measureChildren)
                    InternalChildren[i].Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));

                bool isPinned = (bool)InternalChildren[i].GetValue(PinnedProperty);

                if (isPinned)
                {
                    pinnedChildren.Add(InternalChildren[i]);
                }
                else
                {
                    regularChildren.Add(InternalChildren[i]);
                }
            }

            return new ChildSizeInfos(pinnedChildren, regularChildren);
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            var sortedChildren = GenerateSizeInfo(true);

            Size pinnedItemsSize = new Size();
            if (sortedChildren.PinnedChildren.Count > 0)
                pinnedItemsSize = new Size(sortedChildren.PinnedChildren.Sum(c => c.DesiredSize.Width),
                    sortedChildren.PinnedChildren.Max(c => c.DesiredSize.Height));

            Size regularItemsSize = new Size();
            if (sortedChildren.RegularChildren.Count > 0)
                regularItemsSize = new Size(sortedChildren.RegularChildren.Sum(c => c.DesiredSize.Width),
                    sortedChildren.RegularChildren.Max(c => c.DesiredSize.Height));

            return new Size(Math.Max(pinnedItemsSize.Width, regularItemsSize.Width), Math.Max(this.MinHeight, pinnedItemsSize.Height + regularItemsSize.Height));
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            var sortedChildren = GenerateSizeInfo(false);

            var pinnedChildrenHeight = sortedChildren.PinnedChildren.Any() ? sortedChildren.PinnedChildren.Max(c => c.DesiredSize.Height) : 0.0;
            var regularChildrenHeight = sortedChildren.RegularChildren.Any() ? sortedChildren.RegularChildren.Max(c => c.DesiredSize.Height) : 0.0;

            double x = 0;
            for (int i = 0; i < sortedChildren.PinnedChildren.Count; i++)
            {
                UIElement child = sortedChildren.PinnedChildren[i];
                child.Arrange(new Rect(x, pinnedChildrenHeight - child.DesiredSize.Height, child.DesiredSize.Width, child.DesiredSize.Height));
                x += child.DesiredSize.Width;
            }

            x = 0;
            for (int i = 0; i < sortedChildren.RegularChildren.Count; i++)
            {
                UIElement child = sortedChildren.RegularChildren[i];
                child.Arrange(new Rect(x, pinnedChildrenHeight + regularChildrenHeight - child.DesiredSize.Height, child.DesiredSize.Width, child.DesiredSize.Height));
                x += child.DesiredSize.Width;
            }

            return finalSize;
        }

        #region Pinned attached property

        public static bool GetPinned(DependencyObject obj)
        {
            return (bool)obj.GetValue(PinnedProperty);
        }

        public static void SetPinned(DependencyObject obj, bool value)
        {
            obj.SetValue(PinnedProperty, value);
        }

        // Using a DependencyProperty as the backing store for Pinned.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PinnedProperty =
            DependencyProperty.RegisterAttached("Pinned", typeof(bool), typeof(DocumentTabHeader), new PropertyMetadata(false, HandlePinnedChanged));

        private static void HandlePinnedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var documentTabHeader = VisualTreeHelper.GetParent(d) as DocumentTabHeader;
            if (documentTabHeader != null)
                documentTabHeader.InvalidateMeasure();
        }

        #endregion
    }
}
