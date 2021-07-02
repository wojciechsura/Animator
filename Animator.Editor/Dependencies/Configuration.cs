using Animator.Editor.BusinessLogic.Services.Dialogs;
using Animator.Editor.BusinessLogic.Services.ImageResources;
using Animator.Editor.Services.Dialogs;
using Animator.Editor.Services.ImageResources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;
using Unity.Lifetime;

namespace Animator.Editor.Dependencies
{
    public static class Configuration
    {
        private static bool configured = false;

        public static void Configure(IUnityContainer container)
        {
            if (configured)
                return;

            container.RegisterType<IDialogService, DialogService>(new ContainerControlledLifetimeManager());
            container.RegisterType<IImageResources, ImageResources>(new ContainerControlledLifetimeManager());

            Animator.Editor.BusinessLogic.Dependencies.Configuration.Configure(container);

            configured = true;
        }
    }
}
