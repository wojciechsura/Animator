using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows;

namespace Animator.Designer.Common.Wpf.Extensions
{
    public static class ContextMenuOnLeftClick
    {
        #region PlacementTarget attached property

        public static FrameworkElement GetPlacementTarget(DependencyObject obj)
        {
            return (FrameworkElement)obj.GetValue(PlacementTargetProperty);
        }

        public static void SetPlacementTarget(DependencyObject obj, FrameworkElement value)
        {
            obj.SetValue(PlacementTargetProperty, value);
        }

        // Using a DependencyProperty as the backing store for PlacementTarget.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PlacementTargetProperty =
            DependencyProperty.RegisterAttached("PlacementTarget", typeof(FrameworkElement), typeof(ContextMenuOnLeftClick), new PropertyMetadata(null));

        #endregion

        #region Placement attached property

        public static PlacementMode GetPlacement(DependencyObject obj)
        {
            return (PlacementMode)obj.GetValue(PlacementProperty);
        }

        public static void SetPlacement(DependencyObject obj, PlacementMode value)
        {
            obj.SetValue(PlacementProperty, value);
        }

        // Using a DependencyProperty as the backing store for Placement.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PlacementProperty =
            DependencyProperty.RegisterAttached("Placement", typeof(PlacementMode), typeof(ContextMenuOnLeftClick), new PropertyMetadata(PlacementMode.Bottom));

        #endregion

        #region Enabled attached property

        public static bool GetEnabled(DependencyObject obj)
        {
            return (bool)obj.GetValue(EnabledProperty);
        }

        public static void SetEnabled(DependencyObject obj, bool value)
        {
            obj.SetValue(EnabledProperty, value);
        }

        public static readonly DependencyProperty EnabledProperty = DependencyProperty.RegisterAttached(
            "Enabled",
            typeof(bool),
            typeof(ContextMenuOnLeftClick),
            new UIPropertyMetadata(false, OnEnabledChanged));

        private static void OnEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var uiElement = sender as UIElement;

            if (uiElement != null)
            {
                bool IsEnabled = e.NewValue is bool && (bool)e.NewValue;

                if (IsEnabled)
                {
                    if (uiElement is ButtonBase)
                        ((ButtonBase)uiElement).Click += OnMouseLeftButtonUp;
                    else
                        uiElement.MouseLeftButtonUp += OnMouseLeftButtonUp;
                }
                else
                {
                    if (uiElement is ButtonBase)
                        ((ButtonBase)uiElement).Click -= OnMouseLeftButtonUp;
                    else
                        uiElement.MouseLeftButtonUp -= OnMouseLeftButtonUp;
                }
            }
        }

        private static void OnMouseLeftButtonUp(object sender, RoutedEventArgs e)
        {
            var element = sender as FrameworkElement;
            if (element != null)
            {
                BindingOperations.ClearBinding(element.ContextMenu, ContextMenu.DataContextProperty);
                element.ContextMenu.SetBinding(FrameworkElement.DataContextProperty, new Binding { Source = element.DataContext });

                var target = GetPlacementTarget(element);
                if (target != null)
                    element.ContextMenu.PlacementTarget = target;

                var placement = GetPlacement(element);
                element.ContextMenu.Placement = placement;

                element.ContextMenu.IsOpen = true;
            }
        }

        #endregion
    }
}
