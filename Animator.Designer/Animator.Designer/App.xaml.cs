using Animator.Designer.BusinessLogic.Services.Dialogs;
using Animator.Designer.Dependencies;
using Autofac;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace Animator.Designer
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly IDialogService dialogService;

        private void HandleUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            dialogService.ShowExceptionDialog(e.Exception);
            e.Handled = true;
        }

        public App()
        {
            Container.BuildContainer(Animator.Designer.Dependencies.Configuration.Configure);

            dialogService = Animator.Designer.Dependencies.Container.Instance.Resolve<IDialogService>();
            DispatcherUnhandledException += HandleUnhandledException;
        }
    }
}
