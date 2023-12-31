﻿using Animator.Designer.BusinessLogic.ViewModels.Main;
using Animator.Designer.BusinessLogic.ViewModels.Wrappers;
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

        private List<BaseObjectViewModel> GetVisualHierarchy(BaseObjectViewModel item)
        {
            if (item == null)
                return null;

            // Build hierarchy
            List<BaseObjectViewModel> hierarchy = new List<BaseObjectViewModel>();
            var current = item;
            while (current != null)
            {
                hierarchy.Add(current);
                current = current.VisualParent;
            }

            return hierarchy;
        }

        private TreeViewItem TreeViewItemFromVisualHierarchy(List<BaseObjectViewModel> hierarchy)
        {
            TreeViewItem item = (TreeViewItem)tvHierarchy.ItemContainerGenerator.ContainerFromItem(hierarchy[hierarchy.Count - 1]);

            if (item == null)
                return null;

            for (int i = hierarchy.Count - 2; i >= 0; i--)
            {
                item = (TreeViewItem)item.ItemContainerGenerator.ContainerFromItem(hierarchy[i]);
            }

            return item;
        }

        public void ScrollToItem(List<BaseObjectViewModel> hierarchy)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.ContextIdle,
                new Action(() =>
                {                    
                    var item = TreeViewItemFromVisualHierarchy(hierarchy);
                    item?.BringIntoView();
                }));
        }

        private void HandleGoToRequest(object sender, GoToRequestEventArgs e)
        {
            if (viewModel.SelectedElement != null)
            {
                viewModel.SelectedElement.IsSelected = false;
                viewModel.SelectedElement = null;
            }

            var parents = GetVisualHierarchy(e.Object);

            // Expand all parents

            for (int i = parents.Count - 1; i >= 1; i--)
            {
                parents[i].IsExpanded = true;
            }

            parents[0].IsSelected = true;
            ScrollToItem(parents);
        }

        private void DeinitializeViewModel(DocumentViewModel viewModel)
        {
            movieTimer?.Stop();
            movieTimer = null;

            viewModel.WrapperContext.MovieChanged -= HandleMovieChanged;
            viewModel.WrapperContext.GoToRequest -= HandleGoToRequest;
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
            viewModel.WrapperContext.GoToRequest += HandleGoToRequest;
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
                    if (selected != null)
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

        private void imPreview_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            var image = sender as Image;

            var position = e.GetPosition(image);

            if (image.ActualWidth > 0.0 && image.ActualHeight > 0.0)
            {
                float x = (float)(position.X / image.ActualWidth);
                float y = (float)(position.Y / image.ActualHeight);

                viewModel.NotifyPreviewImageClicked(x, y);
            }
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
