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
using System.Windows.Threading;

namespace Animator.Designer.Controls
{
    public partial class DocumentControl : UserControl, IDisposable
    {
        // Private fields -----------------------------------------------------

        private DispatcherTimer movieTimer;
        private DocumentViewModel viewModel;

        // Private methods ----------------------------------------------------

        private void DeinitializeViewModel(DocumentViewModel viewModel)
        {
            movieTimer?.Stop();
            movieTimer = null;

            viewModel.WrapperContext.MovieChanged -= HandleMovieChanged;
        }

        private void HandleDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (viewModel != null)
            {
                DeinitializeViewModel(viewModel);
                viewModel = null;
            }

            if (e.NewValue != null)
            {
                viewModel = e.NewValue as DocumentViewModel;
                InitializeViewModel(viewModel);
            }
        }

        private void HandleMovieChanged(object sender, EventArgs e)
        {
            movieTimer?.Stop();
            movieTimer?.Start();
        }

        private void InitializeViewModel(DocumentViewModel viewModel)
        {
            movieTimer = new DispatcherTimer(TimeSpan.FromSeconds(2), DispatcherPriority.Background, UpdateMovie, Dispatcher);

            viewModel.WrapperContext.MovieChanged += HandleMovieChanged;
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

        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (viewModel != null)
                viewModel.SelectedElement = e.NewValue as ObjectViewModel;
        }

        private void UpdateMovie(object sender, EventArgs args)
        {
            movieTimer.Stop();
            viewModel.UpdateMovie();
        }

        // Public methods -----------------------------------------------------

        public DocumentControl()
        {
            InitializeComponent();
        }

        public void Dispose()
        {
            movieTimer.Stop();
            movieTimer = null;
        }
    }
}
