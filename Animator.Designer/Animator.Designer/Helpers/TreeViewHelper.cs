using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;

namespace Animator.Designer.Helpers
{
    public static class TreeViewHelper
    {
        public static TreeViewItem VisualUpwardSearch(DependencyObject source)
        {
            while (source != null && !(source is TreeViewItem))
            {
                if (source is Visual)
                    source = VisualTreeHelper.GetParent(source);
                else if (source is FrameworkContentElement element)
                {
                    source = element.Parent;
                }
            }

            return source as TreeViewItem;
        }
    }
}
