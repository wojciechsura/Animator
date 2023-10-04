﻿using Animator.Designer.BusinessLogic.Infrastructure;
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
            XmlDocument xmlDocument = new XmlDocument();

            XmlElement rootNode = document.RootNode.Serialize(xmlDocument);
            xmlDocument.AppendChild(rootNode);

            using FileStream mStream = new FileStream(path, FileMode.Create, FileAccess.Write);
            using XmlTextWriter writer = new XmlTextWriter(mStream, Encoding.Unicode);
            writer.Formatting = Formatting.Indented;

            xmlDocument.Save(writer);
        }

        private void DoNew()
        {
            if (document != null)
            {
                // TODO Ask for saving, possible cancellation etc.
            }

            var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Animator.Designer.BusinessLogic.Resources.EmptyDocument.xml");
            var xmlDocument = new XmlDocument();
            xmlDocument.Load(stream);

            var serializer = new MovieSerializer();
            (var root, var wrapperContext) = serializer.Deserialize(xmlDocument, null);

            Document = new DocumentViewModel(root, wrapperContext);
        }

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
                    (var root, var wrapperContext) = serializer.Deserialize(xmlDocument, path);

                    Document = new DocumentViewModel(root, wrapperContext, path, false);                    
                }
                catch (Exception e)
                {
                    Document = null;

                    messagingService.Warn(String.Format(Strings.Message_FailedToOpenDocument, e.Message));
                }
            }
        }

        private void DoSave()
        {
            if (document.FilenameVirtual)
            {
                DoSaveAs();
                return;
            }

            InternalSaveDocument(document.Filename);
        }

        private void DoSaveAs()
        {
            (bool result, string path) = dialogService.ShowSaveDialog(Strings.FileFilter);

            if (result)
            {
                try
                {
                    InternalSaveDocument(path);

                    document.Filename = path;
                    document.FilenameVirtual = false;
                }
                catch (Exception e)
                {
                    messagingService.ShowError(string.Format(Strings.Message_FailedToSaveDocument, e.Message));
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
