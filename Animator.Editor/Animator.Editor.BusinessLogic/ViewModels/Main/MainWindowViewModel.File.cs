using Animator.Editor.Resources;
using Animator.Editor.BusinessLogic.ViewModels.Document;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace Animator.Editor.BusinessLogic.ViewModels.Main
{
    public partial class MainWindowViewModel
    {
        // Private methods ----------------------------------------------------------

        private static string GenerateBlankFileName(int i)
        {
            return $"{Strings.BlankDocumentName}{i}.xml";
        }

        private void InternalAddDocument(Action<DocumentViewModel> initAction)
        {
            var document = new DocumentViewModel(this);

            initAction(document);

            documents.Add(document);

            ActiveDocument = document;
        }

        private void InternalWriteDocument(DocumentViewModel document, string filename)
        {
            using (var fs = new FileStream(filename, FileMode.Create, FileAccess.Write))
            {
                using (var writer = new StreamWriter(fs))
                {
                    document.Document.WriteTextTo(writer);
                }
            }
        }
       
        private void InternalReadDocument(DocumentViewModel document, string filename)
        {
            using (var fs = new FileStream(filename, FileMode.Open, FileAccess.Read))
            {
                using (var reader = new StreamReader(fs))
                {
                    document.Document.Text = reader.ReadToEnd();
                }
            }
        }

        private bool InternalSaveDocument(DocumentViewModel document, string filename)
        {
            try
            {
                InternalWriteDocument(document, filename);

                document.Document.UndoStack.MarkAsOriginalFile();
                return true;
            }
			catch (Exception e)
            {
                messagingService.ShowError(String.Format(Strings.Message_CannotSaveFile, activeDocument.FileName, e.Message));
                return false;
            }
        }        

        private void InternalLoadDocument(DocumentViewModel document, string filename)
        {
            InternalReadDocument(document, filename);

            document.SetFilename(filename, fileIconProvider.GetImageForFile(filename));
            document.Document.UndoStack.ClearAll();
            document.Document.UndoStack.MarkAsOriginalFile();
            document.FilenameVirtual = false;
            document.Highlighting = highlightingProvider.GetDefinitionByExtension(Path.GetExtension(filename));
        }

        private void LoadDocument(string filename)
        {
            foreach (var document in documents)
            {
                if (string.Equals(document.FileName.ToLower(), filename.ToLower()))
                {
                    ActiveDocument = document;
                    return;
                }
            }

            InternalAddDocument(document =>
            {
                InternalLoadDocument(document, filename);
            });
        }

		private bool SaveDocument(DocumentViewModel document)
        {
            if (document.FilenameVirtual)
                throw new InvalidOperationException("Cannot save document with virtual filename!");

            return InternalSaveDocument(document, document.FileName);
        }

		private bool SaveDocumentAs(DocumentViewModel document)
        {
            var fileDialogResult = dialogService.SaveDialog();
            if (fileDialogResult.Result && InternalSaveDocument(activeDocument, fileDialogResult.FileName))
            {
                activeDocument.SetFilename(fileDialogResult.FileName, fileIconProvider.GetImageForFile(fileDialogResult.FileName));
                activeDocument.FilenameVirtual = false;
                return true;
            }

            return false;
        }

        private void DoNew()
        {
            InternalAddDocument(newDocument =>
            {
                int i = 1;
                while (documents.Any(d => d.FileName.Equals(GenerateBlankFileName(i))))
                    i++;

                string newFilename = GenerateBlankFileName(i);
                newDocument.SetFilename(newFilename, fileIconProvider.GetImageForFile(newFilename));
                newDocument.Highlighting = highlightingProvider.GetDefinitionByExtension(".xml");

                // Fill with template
                string template;
                using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Animator.Editor.BusinessLogic.Resources.NewDocumentTemplate.xml"))
                using (var sr = new StreamReader(stream))
                    template = sr.ReadToEnd();

                newDocument.Document.Text = template;
            });            
        }

        private void DoOpen()
        {
            var dialogResult = dialogService.OpenDialog();
            if (dialogResult.Result)
            {
                try
                {
                    LoadDocument(dialogResult.FileName);                    
                }
                catch (Exception e)
                {
                    messagingService.ShowError(string.Format(Strings.Message_CannotOpenFile, dialogResult.FileName, e.Message));
                }
            }
        }

        private void DoSave()
        {
            if (activeDocument.FilenameVirtual)
                DoSaveAs();
            else
                SaveDocument(activeDocument);
        }

        private void DoSaveAs()
        {
            SaveDocumentAs(activeDocument);
        }
    }
}
