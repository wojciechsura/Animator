using Animator.Designer.BusinessLogic.ViewModels.MacroPropertyName;
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
    /// Logika interakcji dla klasy MacroPropertyNameWindow.xaml
    /// </summary>
    public partial class MacroPropertyNameWindow : Window, IMacroPropertyNameWindowAccess
    {
        private readonly MacroPropertyNameWindowViewModel viewModel;

        void IMacroPropertyNameWindowAccess.Close(bool result)
        {
            DialogResult = result;
            Close();
        }

        public MacroPropertyNameWindow(List<string> existingNames)
        {
            InitializeComponent();

            viewModel = Dependencies.Container.Instance.Resolve<MacroPropertyNameWindowViewModel>(new NamedParameter("access", this), new NamedParameter("existingNames", existingNames));
            DataContext = viewModel;
        }

        public string Result => viewModel.Name;
    }
}
