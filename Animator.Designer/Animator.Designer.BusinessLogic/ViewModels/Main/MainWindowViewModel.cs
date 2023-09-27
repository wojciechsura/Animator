using Animator.Designer.BusinessLogic.Infrastructure;
using Animator.Designer.BusinessLogic.Services.Dialogs;
using Animator.Designer.BusinessLogic.Services.Messaging;
using Animator.Designer.BusinessLogic.ViewModels.Base;
using Animator.Designer.Resources.Windows.MainWindow;
using Spooksoft.VisualStateManager.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml;

namespace Animator.Designer.BusinessLogic.ViewModels.Main
{
    public class MainWindowViewModel : BaseViewModel
    {
        // Private fields -----------------------------------------------------

        private readonly IMainWindowAccess access;
        private readonly IDialogService dialogService;
        private readonly IMessagingService messagingService;
        private DocumentViewModel document;

        // Private methods ----------------------------------------------------

        private void DoOpen()
        {
            (bool result, string path) = dialogService.ShowOpenDialog(Strings.FileFilter);
            
            if (result)
            {
                if (document != null)
                {
                    // TODO Ask for saving, possible cancellation etc.
                }

                try
                {
                    var xmlDocument = new XmlDocument();
                    xmlDocument.Load(path);

                    var serializer = new MovieSerializer();
                    var root = serializer.Deserialize(xmlDocument, path);

                    Document = new DocumentViewModel(root, path, false);
                }
                catch (Exception e)
                {
                    Document = null;

                    messagingService.Warn(String.Format(Strings.Message_FailedToOpenDocument, e.Message));
                }
            }
        }

        // Public methods -----------------------------------------------------

        public MainWindowViewModel(IMainWindowAccess access, 
            IDialogService dialogService,
            IMessagingService messagingService)
        {
            this.access = access;
            this.dialogService = dialogService;
            this.messagingService = messagingService;

            OpenCommand = new AppCommand(obj => DoOpen());
        }

        // Public properties --------------------------------------------------

        public ICommand OpenCommand { get; }

        public DocumentViewModel Document
        {
            get => document;
            set => Set(ref document, value);
        }
    }
}
