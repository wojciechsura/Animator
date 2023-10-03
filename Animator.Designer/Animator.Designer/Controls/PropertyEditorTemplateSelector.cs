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
            else if (item is MultilineStringPropertyViewModel)
                return MultilineStringPropertyTemplate;
            else if (item is ManagedSimplePropertyViewModel)
                return ManagedSimplePropertyTemplate;
            else if (item is ManagedCollectionPropertyViewModel)
                return ManagedCollectionPropertyTemplate;
            else if (item is ManagedReferencePropertyViewModel)
                return ManagedReferencePropertyTemplate;           
            else if (item is ReferencePropertyViewModel) 
                return ReferencePropertyTemplate;

            return null;
        }

        public DataTemplate StringPropertyTemplate { get; set; }

        public DataTemplate MultilineStringPropertyTemplate { get; set; }

        public DataTemplate ManagedSimplePropertyTemplate { get; set; }

        public DataTemplate ManagedCollectionPropertyTemplate { get; set; }
        
        public DataTemplate ManagedReferencePropertyTemplate { get; set; }

        public DataTemplate ReferencePropertyTemplate { get; set; }
    }
}
