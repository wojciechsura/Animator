using Animator.Editor.BusinessLogic.ViewModels.Document;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Animator.Editor.Wpf
{
    public class DocumentTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (container is FrameworkElement element && item is DocumentViewModel)
            {
                return element.FindResource("TextDocumentTemplate") as DataTemplate;
            }

            return null;
        }
    }
}
