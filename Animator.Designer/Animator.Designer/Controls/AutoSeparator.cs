using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Windows;

namespace Animator.Designer.Controls
{
    public class AutoSeparator : Separator
    {
        public AutoSeparator()
        {
            if (DesignerProperties.GetIsInDesignMode(this))
                return;

            Visibility = Visibility.Collapsed; // Starting collapsed so we don't see them disappearing

            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(UpdateVisibility), DispatcherPriority.Render);
        }

        private void UpdateVisibility()
        {
            ItemCollection items = ((ItemsControl)Parent).Items;
            int index = items.IndexOf(this);

            if (index == -1)
                return;

            int i;
            for (i = index - 1; i >= 0; i--)
            {
                if (items[i] is UIElement uiElement)
                {
                    // Invisible item cannot be valid predecessor
                    if (uiElement.Visibility != Visibility.Visible)
                        continue;
                    // Separator is invalid predecessor
                    else if (uiElement is Separator or AutoSeparator)
                        return;
                    // Anything else is valid predecessor
                    else
                        break;
                }
            }

            // No valid item found before separator
            if (i < 0)
                return;

            for (i = index + 1; i < items.Count; i++)
            {
                if (items[i] is UIElement uiElement)
                {
                    // Invisible item cannot be valid successor
                    if (uiElement.Visibility != Visibility.Visible)
                        continue;
                    // Separator is invalid successor
                    else if (uiElement is Separator or AutoSeparator)
                        return;
                    // Anything else is valid successor
                    else
                        break;
                }
            }

            if (i >= items.Count)
                return;

            Visibility = Visibility.Visible;
        }
    }
}
