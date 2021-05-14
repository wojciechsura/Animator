using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Animator.Editor.Wpf
{
    public static class ParentData
    {
        #region ParentItemsSource dependency property

        public static object GetParentItemsSource(DependencyObject obj)
        {
            return (object)obj.GetValue(ParentItemsSourceProperty);
        }

        public static void SetParentItemsSource(DependencyObject obj, object value)
        {
            obj.SetValue(ParentItemsSourceProperty, value);
        }

        // Using a DependencyProperty as the backing store for ParentItemsSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ParentItemsSourceProperty =
            DependencyProperty.RegisterAttached("ParentItemsSource", typeof(object), typeof(ParentData), new PropertyMetadata(null));

        #endregion
    }
}
