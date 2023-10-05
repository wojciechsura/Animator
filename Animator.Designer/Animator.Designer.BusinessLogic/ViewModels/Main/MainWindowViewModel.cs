using Animator.Designer.BusinessLogic.Infrastructure;
using Animator.Designer.BusinessLogic.Services.Dialogs;
using Animator.Designer.BusinessLogic.Services.Messaging;
using Animator.Designer.BusinessLogic.ViewModels.Base;
using Animator.Designer.BusinessLogic.ViewModels.Wrappers;
using Animator.Designer.Resources.Windows.MainWindow;
using Irony.Parsing.Construction;
using Spooksoft.VisualStateManager.Commands;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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

        private void InternalSaveDocument(string path)
        {
            var fs = document.Save(() => new FileStream(path, FileMode.Create, FileAccess.Write));
            fs.Dispose();
        }

        private void DoNew()
        {
            if (document != null)
            {
                var answer = messagingService.AskYesNoCancel(Animator.Designer.Resources.Windows.MainWindow.Strings.Message_SaveBeforeClose);

                if (answer == true)
                {
                    if (!DoSave())
                        return false;
                }
                else if (answer == null)
                {
                    return false;
                }
            }

            var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Animator.Designer.BusinessLogic.Resources.EmptyDocument.xml");
            var xmlDocument = new XmlDocument();
            xmlDocument.Load(stream);

            var serializer = new MovieSerializer();
            (var root, var wrapperContext) = serializer.Deserialize(xmlDocument, null);

            Document = new DocumentViewModel(root, wrapperContext);
        }

        private bool DoOpen()
        {
            (bool result, string path) = dialogService.ShowOpenDialog(Strings.FileFilter);
            
            if (result)
            {
                if (document != null)
                {
                    var answer = messagingService.AskYesNoCancel(Animator.Designer.Resources.Windows.MainWindow.Strings.Message_SaveBeforeClose);

                    if (answer == true)
                    {
                        if (!DoSave())
                            return false;
                    }
                    else if (answer == null)
                    {
                        return false;
                    }
                }

                try
                {
                    var xmlDocument = new XmlDocument();
                    xmlDocument.Load(path);

                    var serializer = new MovieSerializer();
                    (var root, var wrapperContext) = serializer.Deserialize(xmlDocument, path);

                    Document = new DocumentViewModel(root, wrapperContext, path, false);
                    return true;
                }
                catch (Exception e)
                {
                    Document = null;

                    messagingService.Warn(String.Format(Strings.Message_FailedToOpenDocument, e.Message));
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        private bool DoSave()
        {
            if (document.FilenameVirtual)
            {
                return DoSaveAs();                
            }

            try
            {
                InternalSaveDocument(document.Filename);
                return true;
            }
            catch (Exception e)
            {
                messagingService.ShowError(string.Format(Strings.Message_FailedToSaveDocument, e.Message));
                return false;
            }
        }

        private bool DoSaveAs()
        {
            (bool result, string path) = dialogService.ShowSaveDialog(Strings.FileFilter);

            if (result)
            {
                try
                {
                    InternalSaveDocument(path);

                    document.Filename = path;
                    document.FilenameVirtual = false;

                    return true;
                }
                catch (Exception e)
                {
                    messagingService.ShowError(string.Format(Strings.Message_FailedToSaveDocument, e.Message));
                    return false;
                }
            }

            return false;
        }

        // Public methods -----------------------------------------------------

        public MainWindowViewModel(IMainWindowAccess access, 
            IDialogService dialogService,
            IMessagingService messagingService)
        {
            this.access = access;
            this.dialogService = dialogService;
            this.messagingService = messagingService;

            NewCommand = new AppCommand(obj => DoNew());
            OpenCommand = new AppCommand(obj => DoOpen());
            SaveCommand = new AppCommand(obj => DoSave());
            SaveAsCommand = new AppCommand(obj => DoSaveAs());

            DoNew();
        }

        // Public properties --------------------------------------------------

        public ICommand NewCommand { get; }
        public ICommand OpenCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand SaveAsCommand { get; }

        public DocumentViewModel Document
        {
            get => document;
            set => Set(ref document, value);
        }
    }
}
