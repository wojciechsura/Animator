using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Animator.Editor.Wpf
{
    public static class MouseEvents
    {
        #region DirectMouseDoubleClickCommand attached property


        public static ICommand GetDirectMouseDoubleClickCommand(DependencyObject obj)
        {
            return (ICommand)obj.GetValue(DirectMouseDoubleClickCommandProperty);
        }

        public static void SetDirectMouseDoubleClickCommand(DependencyObject obj, ICommand value)
        {
            obj.SetValue(DirectMouseDoubleClickCommandProperty, value);
        }

        // Using a DependencyProperty as the backing store for DirectMouseDoubleClickCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DirectMouseDoubleClickCommandProperty =
            DependencyProperty.RegisterAttached("DirectMouseDoubleClickCommand", typeof(ICommand), typeof(MouseEvents), new PropertyMetadata(null, HandleDirectMouseDoubleClickCommandChanged));

        private static void HandleDirectMouseDoubleClickCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Control control)
            {
                control.MouseDoubleClick -= HandleMouseDoubleClick;

                if (e.NewValue != null)
                {
                    control.MouseDoubleClick += HandleMouseDoubleClick;
                }
            }
        }

        private static void HandleMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            // Only direct doubleclick is handled
            if (sender is DependencyObject dependencyObject && sender == e.OriginalSource)
            {
                ICommand command = GetDirectMouseDoubleClickCommand(dependencyObject);

                if (command?.CanExecute(null) ?? false)
                    command?.Execute(null);
            }
        }

        #endregion
    }
}
