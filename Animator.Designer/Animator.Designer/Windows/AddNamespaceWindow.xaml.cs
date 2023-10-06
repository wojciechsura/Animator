using Animator.Designer.BusinessLogic.Models.AddNamespace;
using Animator.Designer.BusinessLogic.ViewModels.AddNamespace;
using Animator.Designer.BusinessLogic.ViewModels.Wrappers;
using Autofac;
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
using System.Windows.Shapes;

namespace Animator.Designer.Windows
{
    /// <summary>
    /// Logika interakcji dla klasy AddNamespaceWindow.xaml
    /// </summary>
    public partial class AddNamespaceWindow : Window, IAddNamespaceWindowAccess
    {
        private readonly AddNamespaceWindowViewModel viewModel;

        void IAddNamespaceWindowAccess.Close(bool result)
        {
            DialogResult = result;
            Close();
        }

        public AddNamespaceWindow(WrapperContext context)
        {
            InitializeComponent();

            viewModel = Dependencies.Container.Instance.Resolve<AddNamespaceWindowViewModel>(new NamedParameter("wrapperContext", context), new NamedParameter("access", this));
            DataContext = viewModel;
        }

        public AddNamespaceResultModel Result => viewModel.Result;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.MinWidth = this.ActualWidth;
            this.MinHeight = this.ActualHeight;
            this.MaxHeight = this.ActualHeight;
        }
    }
}
