using Animator.Designer.BusinessLogic.Services.Dialogs;
using Animator.Designer.BusinessLogic.Services.Messaging;
using Animator.Designer.Services.DialogService;
using Animator.Designer.Services.Messaging;
using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Designer.Dependencies
{
    public static class Configuration
    {
        private static bool configured = false;

        public static void Configure(ContainerBuilder builder)
        {
            if (configured)
                return;

            builder.RegisterType<MessagingService>().As<IMessagingService>().SingleInstance();
            builder.RegisterType<DialogService>().As<IDialogService>().SingleInstance();

            Animator.Designer.BusinessLogic.Dependencies.Configuration.Configure(builder);

            configured = true;
        }
    }
}
