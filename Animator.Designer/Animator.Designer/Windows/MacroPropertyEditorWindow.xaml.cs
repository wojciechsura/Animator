using Animator.Designer.BusinessLogic.ViewModels.MacroPropertyEditor;
using Animator.Designer.BusinessLogic.ViewModels.Wrappers.Objects;
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
    /// Logika interakcji dla klasy MacroPropertyEditor.xaml
    /// </summary>
    public partial class MacroPropertyEditorWindow : Window, IMacroPropertyEditorWindowAccess
    {
        private readonly MacroPropertyEditorWindowViewModel viewModel;

        void IMacroPropertyEditorWindowAccess.Close(bool result)
        {
            DialogResult = result;
            Close();
        }

        public MacroPropertyEditorWindow(MacroViewModel editedMacro)
        {
            InitializeComponent();

            viewModel = Dependencies.Container.Instance.Resolve<MacroPropertyEditorWindowViewModel>(new NamedParameter("editedMacro", editedMacro), new NamedParameter("access", this));
            DataContext = viewModel;
        }
    }
}
