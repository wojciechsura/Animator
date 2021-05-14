using Animator.Editor.BusinessLogic.Services.StartupInfo;
using Animator.Editor.Dependencies;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Unity;

namespace Animator.Editor
{
    /// <summary>
    /// Logika interakcji dla klasy App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            Dependencies.Configuration.Configure(Container.Instance);            
        }

        private void HandleApplicationStartup(object sender, StartupEventArgs e)
        {
            var startupService = Container.Instance.Resolve<IStartupInfoService>();
            startupService.Parameters = e.Args;
        }
    }
}
