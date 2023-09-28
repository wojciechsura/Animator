using Animator.Designer.BusinessLogic.ViewModels.Wrappers.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Animator.Designer.Controls
{
    public class PropertyEditorTemplateSelector : DataTemplateSelector
    {
        public PropertyEditorTemplateSelector()
        {

        }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is StringPropertyViewModel)
                return StringPropertyTemplate;

            return null;
        }

        public DataTemplate StringPropertyTemplate { get; set; }
    }
}
