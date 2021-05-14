using Animator.Editor.BusinessLogic.Services.Commands;
using Animator.Editor.BusinessLogic.Services.Config;
using Animator.Editor.BusinessLogic.Services.Dialogs;
using Animator.Editor.BusinessLogic.Services.FileIcons;
using Animator.Editor.BusinessLogic.Services.Highlighting;
using Animator.Editor.BusinessLogic.Services.Messaging;
using Animator.Editor.BusinessLogic.Services.Paths;
using Animator.Editor.BusinessLogic.Services.StartupInfo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;
using Unity.Lifetime;

namespace Animator.Editor.BusinessLogic.Dependencies
{
    public static class Configuration
    {
        public static void Configure(IUnityContainer container)
        {
            container.RegisterType<IMessagingService, MessagingService>(new ContainerControlledLifetimeManager());
            container.RegisterType<IConfigurationService, ConfigrationService>(new ContainerControlledLifetimeManager());
            container.RegisterType<IPathService, PathService>(new ContainerControlledLifetimeManager());
            container.RegisterType<IStartupInfoService, StartupInfoService>(new ContainerControlledLifetimeManager());
            container.RegisterType<IHighlightingProvider, HighlightingProvider>(new ContainerControlledLifetimeManager());
            container.RegisterType<ICommandRepositoryService, CommandRepositoryService>(new ContainerControlledLifetimeManager());
            container.RegisterType<IFileIconProvider, FileIconProvider>(new ContainerControlledLifetimeManager());
        }
    }
}
