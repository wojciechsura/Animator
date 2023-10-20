using Animator.Designer.BusinessLogic.Infrastructure;
using Animator.Designer.BusinessLogic.Models.AddNamespace;
using Animator.Designer.BusinessLogic.Services.Dialogs;
using Animator.Designer.BusinessLogic.Services.Messaging;
using Animator.Designer.BusinessLogic.ViewModels.Base;
using Animator.Designer.BusinessLogic.ViewModels.Wrappers;
using Animator.Designer.Resources.Windows.MainWindow;
using Animator.Engine.Base.Extensions;
using Animator.Engine.Base.Persistence;
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
            document.Changed = false;
        }

        private bool DoNew()
        {
            if (document != null && document.Changed)
            {
                var answer = messagingService.AskYesNoCancel(string.Format(Strings.Message_SaveBeforeClose, document.Filename));

                if (answer == true)
                {
                    if (!DoSave())
                        return false;
                }
                else if (answer == null)
                {
                    return false;
                }

                document.Dispose();
                Document = null;
            }

            var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Animator.Designer.BusinessLogic.Resources.EmptyDocument.xml");
            var xmlDocument = new XmlDocument();
            xmlDocument.Load(stream);

            var serializer = new MovieSerializer();
            (var root, var wrapperContext) = serializer.Deserialize(xmlDocument);

            Document = new DocumentViewModel(dialogService, root, wrapperContext);

            return true;
        }

        private bool DoOpen()
        {
            (bool result, string path) = dialogService.ShowOpenDialog(Strings.FileFilter);
            
            if (result)
            {
                if (document != null && document.Changed)
                {
                    var answer = messagingService.AskYesNoCancel(string.Format(Strings.Message_SaveBeforeClose, document.Filename));

                    if (answer == true)
                    {
                        if (!DoSave())
                            return false;
                    }
                    else if (answer == null)
                    {
                        return false;
                    }

                    document.Dispose();
                    Document = null;
                }

                try
                {
                    var xmlDocument = new XmlDocument();
                    xmlDocument.Load(path);

                    var serializer = new MovieSerializer();
                    (var root, var wrapperContext) = serializer.Deserialize(xmlDocument);

                    Document = new DocumentViewModel(dialogService, root, wrapperContext, path, false);
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

        private void DoAddNamespace()
        {
            (bool result, AddNamespaceResultModel model) = dialogService.ShowAddNamespaceDialog(document.WrapperContext);
            if (result)
            {
                var uri = new NamespaceDefinition(model.Assembly.GetName().Name, model.Namespace).ToString();
                var nvm = new AssemblyNamespaceViewModel(model.Prefix, uri, model.Assembly, model.Namespace);
                document.WrapperContext.AddNamespace(nvm);

                model.Assembly.InitializeStaticTypes(model.Namespace);

                document.NotifyAvailableNamespacesChanged();
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

            NewCommand = new AppCommand(obj => DoNew());
            OpenCommand = new AppCommand(obj => DoOpen());
            SaveCommand = new AppCommand(obj => DoSave());
            SaveAsCommand = new AppCommand(obj => DoSaveAs());
            AddNamespaceCommand = new AppCommand(obj => DoAddNamespace());

            DoNew();
        }

        public void Close()
        {
            if (document != null)
                document.Dispose();            
        }

        // Public properties --------------------------------------------------

        public ICommand NewCommand { get; }
        public ICommand OpenCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand SaveAsCommand { get; }
        public ICommand AddNamespaceCommand { get; }

        public DocumentViewModel Document
        {
            get => document;
            set => Set(ref document, value);
        }
    }
}
