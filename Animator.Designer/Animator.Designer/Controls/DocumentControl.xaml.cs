using Animator.Designer.BusinessLogic.ViewModels.Main;
using Animator.Designer.BusinessLogic.ViewModels.Wrappers.Objects;
using Animator.Designer.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Animator.Designer.Controls
{
    /// <summary>
    /// Logika interakcji dla klasy DocumentControl.xaml
    /// </summary>
    public partial class DocumentControl : UserControl
    {
        public DocumentControl()
        {
            InitializeComponent();
        }

        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            (DataContext as DocumentViewModel).SelectedElement = e.NewValue as ObjectViewModel;
        }

        private void TreeView_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Right)
            {
                TreeViewItem treeViewItem = TreeViewHelper.VisualUpwardSearch(e.OriginalSource as DependencyObject);

                if (treeViewItem != null)
                {
                    treeViewItem.IsSelected = true;
                }
                else
                {
                    BaseObjectViewModel selected = (sender as TreeView).SelectedItem as BaseObjectViewModel;
                    selected.IsSelected = true;
                }
            }
        }
    }
}
