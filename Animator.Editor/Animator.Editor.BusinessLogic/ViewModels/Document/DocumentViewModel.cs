using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml;
using Animator.Editor.BusinessLogic.Models.Documents;
using Animator.Editor.BusinessLogic.Models.Highlighting;
using Animator.Editor.BusinessLogic.Models.Search;
using Animator.Editor.BusinessLogic.ViewModels.Base;
using Animator.Editor.Common.Commands;
using Animator.Editor.Common.Conditions;
using Animator.Engine.Elements;
using Animator.Engine.Elements.Persistence;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Document;

namespace Animator.Editor.BusinessLogic.ViewModels.Document
{
    public class DocumentViewModel : BaseViewModel
    {
        // Private classes ----------------------------------------------------

        private class UpdateMovieInput
        {
            public UpdateMovieInput(string movieXml)
            {
                MovieXml = movieXml;
            }

            public string MovieXml { get; }
        }

        private class MovieUpdatedResult
        {
            public MovieUpdatedResult(Movie animation)
            {
                Movie = animation;
            }

            public Movie Movie { get; }
        }

        private class MovieUpdateFailed
        {
            public Exception Exception { get; }

            public MovieUpdateFailed(Exception exception)
            {
                Exception = exception;
            }
        }

        private class UpdateMovieWorker : BackgroundWorker
        {
            private static readonly MovieSerializer movieSerializer = new();

            protected override void OnDoWork(DoWorkEventArgs e)
            {
                var input = e.Argument as UpdateMovieInput;

                Movie movie = null;
                try
                {
                    XmlDocument document = new XmlDocument();
                    document.LoadXml(input.MovieXml);

                    movie = movieSerializer.Deserialize(document);
                    e.Result = new MovieUpdatedResult(movie);
                }
                catch (Exception ex)
                {
                    e.Result = new MovieUpdateFailed(ex);
                }
            }

            public UpdateMovieWorker()
            {
                WorkerSupportsCancellation = true;
            }
        }

        // Private fields -----------------------------------------------------

        private readonly TextDocument document;
        private readonly IDocumentHandler handler;

        private bool changed;
        private bool filenameVirtual;
        private bool canUndo;
        private bool canRedo;
        private bool selectionAvailable;
        private bool regularSelectionAvailable;
        private bool isPinned;

        private DocumentState storedState;

        private IEditorAccess editorAccess;
        private SearchModel lastSearch;
        private HighlightingInfo highlighting;
        private ImageSource icon;

        private Movie movie;
        private UpdateMovieWorker updateMovieWorker;

        // Private methods ----------------------------------------------------

        private void HandleFileNameChanged(object sender, EventArgs e)
        {
            OnPropertyChanged(() => FileName);
        }

        private void HandleUndoStackPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(document.UndoStack.CanUndo))
                this.CanUndo = document.UndoStack.CanUndo;
            else if (e.PropertyName == nameof(document.UndoStack.CanRedo))
                this.CanRedo = document.UndoStack.CanRedo;
            else if (e.PropertyName == nameof(document.UndoStack.IsOriginalFile))
                this.Changed = !document.UndoStack.IsOriginalFile;
        }

        private void ValidateEditorAccess()
        {
            if (editorAccess == null)
                throw new InvalidOperationException("No editor attached!");
        }

        private void DoClose()
        {
            handler.RequestClose(this);
        }

        private void UpdateMovieFinished(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Result is MovieUpdatedResult movieUpdated)
            {
                movie = movieUpdated.Movie;

                System.Diagnostics.Debug.WriteLine("Movie update succeeded");

                // TODO update timeline ranges
                // TODO render current frame
            }
            else if (e.Result is MovieUpdateFailed movieUpdateFailed)
            {
                System.Diagnostics.Debug.WriteLine("Movie update failed");

                // TODO display errors in appropriate place
            }
        }

        // Public methods -----------------------------------------------------

        public DocumentViewModel(IDocumentHandler handler)
        {
            this.handler = handler;

            CloseCommand = new AppCommand(obj => DoClose());

            document = new TextDocument();
            document.FileNameChanged += HandleFileNameChanged;
            document.UndoStack.PropertyChanged += HandleUndoStackPropertyChanged;

            editorAccess = null;

            canUndo = document.UndoStack.CanUndo;
            canRedo = document.UndoStack.CanRedo;
            changed = document.UndoStack.IsOriginalFile;
            selectionAvailable = false;

            storedState = null;
            changed = false;
            filenameVirtual = true;

            lastSearch = null;
        }

        public void RunAsSingleHistoryEntry(Action action)
        {
            try
            {
                editorAccess.BeginChange();
                action();
            }
            finally
            {
                editorAccess.EndChange();
            }
        }

        public void NotifySelectionAvailable(bool selectionAvailable) => SelectionAvailable = selectionAvailable;

        public void NotifyRegularSelectionAvailable(bool regularSelectionAvailable) => RegularSelectionAvailable = regularSelectionAvailable;        

        public DocumentState LoadState()
        {
            return storedState;
        }

        public void SaveState(DocumentState state)
        {
            storedState = state;
        }

        public void Copy()
        {
            ValidateEditorAccess();
            editorAccess.Copy();
        }

        public void Cut()
        {
            ValidateEditorAccess();
            editorAccess.Cut();
        }

        public void Paste()
        {
            ValidateEditorAccess();
            editorAccess.Paste();
        }

        public (int selStart, int selLength) GetSelection()
        {
            return editorAccess.GetSelection();
        }

        public void SetSelection(int selStart, int selLength, bool scrollTo = true)
        {
            editorAccess.SetSelection(selStart, selLength);

            if (scrollTo)
            {
                var location = document.GetLocation(selStart);
                editorAccess.ScrollTo(location.Line, location.Column);
            }
        }

        public string GetSelectedText()
        {
            return editorAccess.GetSelectedText();
        }

        public void FocusDocument()
        {
            editorAccess.FocusDocument();
        }

        public void UpdateMovie()
        {
            if (updateMovieWorker != null && updateMovieWorker.IsBusy)
            {
                updateMovieWorker.CancelAsync();
                updateMovieWorker = null;
            }

            var text = document.Text;
            var argument = new UpdateMovieInput(text);

            updateMovieWorker = new UpdateMovieWorker();
            updateMovieWorker.RunWorkerCompleted += UpdateMovieFinished;
            updateMovieWorker.RunWorkerAsync(argument);
        }

        // Public properties --------------------------------------------------

        public ICommand CloseCommand { get; }

        public TextDocument Document => document;

        public void SetFilename(string filename, ImageSource icon)
        {
            document.FileName = filename;
            this.icon = icon;

            OnPropertyChanged(() => FileName);
            OnPropertyChanged(() => Title);
            OnPropertyChanged(() => Icon);
        }

        public string FileName
        {
            get => document.FileName;
        }

        public ImageSource Icon
        {
            get => icon;
        }

        public string Title => System.IO.Path.GetFileName(document.FileName);
            
        public bool Changed
        {
            get => changed;
            set => Set(ref changed, () => Changed, value);
        }

        public bool FilenameVirtual
        {
            get => filenameVirtual;
            set => Set(ref filenameVirtual, () => FilenameVirtual, value);
        }

        public bool CanUndo
        {
            get => canUndo;
            set => Set(ref canUndo, () => CanUndo, value);
        }

        public bool CanRedo
        {
            get => canRedo;
            set => Set(ref canRedo, () => CanRedo, value);
        }

        public bool SelectionAvailable
        {
            get => selectionAvailable;
            set => Set(ref selectionAvailable, () => SelectionAvailable, value);
        }

        public bool RegularSelectionAvailable
        {
            get => regularSelectionAvailable;
            set => Set(ref regularSelectionAvailable, () => RegularSelectionAvailable, value);
        }

        public HighlightingInfo Highlighting
        {
            get => highlighting;
            set => Set(ref highlighting, () => Highlighting, value);
        }

        public IDocumentHandler Handler => handler;

        public IEditorAccess EditorAccess
        {
            get => editorAccess;
            set => editorAccess = value;
        }

        public SearchModel LastSearch
        {
            get => lastSearch;
            set => Set(ref lastSearch, () => LastSearch, value);
        }

        public bool IsPinned
        {
            get => isPinned;
            set => Set(ref isPinned, () => IsPinned, value);
        }
    }
}
