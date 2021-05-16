﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml;
using Animator.Editor.BusinessLogic.Models.Documents;
using Animator.Editor.BusinessLogic.Models.Highlighting;
using Animator.Editor.BusinessLogic.Models.Search;
using Animator.Editor.BusinessLogic.ViewModels.Base;
using Animator.Editor.Common.Commands;
using Animator.Editor.Common.Conditions;
using Animator.Engine.Base;
using Animator.Engine.Base.Exceptions;
using Animator.Engine.Elements;
using Animator.Engine.Elements.Persistence;
using Animator.Engine.Exceptions;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Document;
using Microsoft.Language.Xml;

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

        private class FrameRenderInput
        {
            public FrameRenderInput(Movie movie, int frameIndex)
            {
                Movie = movie;
                FrameIndex = frameIndex;
            }

            public Movie Movie { get; }
            public int FrameIndex { get; }
        }

        private class FrameRenderedResult
        {
            public FrameRenderedResult(Bitmap frame)
            {
                Frame = frame;
            }

            public Bitmap Frame { get; }
        }

        private class FrameRenderingFailure
        {
            public FrameRenderingFailure(Exception exception)
            {
                Exception = exception;
            }

            public Exception Exception { get; }
        }

        private class FrameRendererWorker : BackgroundWorker
        {
            private Bitmap RenderFrameAt(Movie movie, TimeSpan time)
            {
                if (movie.Scenes.Count == 0)
                    throw new InvalidOperationException("No scenes to render!");

                TimeSpan summedTime = TimeSpan.FromSeconds(0);

                int i = 0;
                while (i < movie.Scenes.Count && summedTime + movie.Scenes[i].Duration < time)
                {
                    summedTime += movie.Scenes[i].Duration;
                    i++;
                }

                if (i >= movie.Scenes.Count)
                    throw new InvalidOperationException("Given time exceedes whole movie time!");

                TimeSpan sceneTimeOffset = time - summedTime;

                movie.Scenes[i].ApplyAnimation((float)sceneTimeOffset.TotalMilliseconds);

                var result = new Bitmap(movie.Config.Width, movie.Config.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                movie.Scenes[i].Render(result);

                return result;
            }

            protected override void OnDoWork(DoWorkEventArgs e)
            {
                var input = e.Argument as FrameRenderInput;

                try
                {
                    var framesPerSecond = input.Movie.Config.FramesPerSecond;
                    TimeSpan time = TimeSpan.FromSeconds(1 / framesPerSecond * input.FrameIndex);
                    Bitmap result = RenderFrameAt(input.Movie, time);

                    e.Result = new FrameRenderedResult(result);
                }
                catch (Exception ex)
                {
                    e.Result = new FrameRenderingFailure(ex);
                }
            }

            public FrameRendererWorker()
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
        private BitmapSource frame;
        private FrameRendererWorker frameRendererWorker;
        private int minFrame;
        private int frameIndex;
        private int maxFrame;
        private string parsingError;
        private string renderingError;

        // Private methods ----------------------------------------------------

        [DllImport("gdi32")]
        private static extern int DeleteObject(IntPtr o);

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

        private string BuildError(string message, Exception exception)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(message);
            sb.AppendLine(Resources.Strings.Error_Reason);
            do
            {
                if (exception is ParseException parseException)
                    sb.AppendLine($"    {parseException.Message}");
                else if (exception is SerializerException serializerException)
                    sb.AppendLine($"    {serializerException.Message} {Resources.Strings.Error_At} {serializerException.XPath}");
                else if (exception is AnimationException animationException)
                    sb.AppendLine($"    {animationException.Message} {Resources.Strings.Error_At} {animationException.Path}");
                else
                    sb.AppendLine($"    {exception.Message}");

                exception = exception.InnerException;
                if (exception != null)
                    sb.AppendLine(Resources.Strings.Error_Reason);
            }
            while (exception != null);

            return sb.ToString();
        }

        private string BuildErrors(params string[] errors)
        {
            var sb = new StringBuilder();
            bool first = true;

            foreach (var error in errors)
            {
                if (first)
                    first = false;
                else
                    sb.AppendLine();

                sb.AppendLine(error);
            }

            return sb.ToString();
        }

        private void UpdateMovieFinished(object sender, RunWorkerCompletedEventArgs e)
        {
            ParsingError = null;
            RenderingError = null;

            if (e.Cancelled)
                return;

            if (e.Result is MovieUpdatedResult movieUpdated)
            {
                movie = movieUpdated.Movie;

                MinFrame = 0;

                if (movie.Scenes.Count > 0)
                {
                    MaxFrame = (int)(movie.Scenes.Sum(s => s.Duration.TotalSeconds) * movie.Config.FramesPerSecond) - 1;
                }
                else
                {
                    MaxFrame = 0;
                }

                FrameIndex = Math.Max(MinFrame, Math.Min(maxFrame, FrameIndex));
            }
            else if (e.Result is MovieUpdateFailed movieUpdateFailed)
            {
                ParsingError = BuildError(Resources.Strings.Error_ParsingFailed, movieUpdateFailed.Exception);
            }
        }

        private void HandleFrameIndexChanged()
        {
            UpdateFrame();
        }

        private void HandleErrorChanged()
        {
            OnPropertyChanged(() => Error);
            OnPropertyChanged(() => ShowError);
        }

        private void FrameRendered(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
                return;

            RenderingError = null;

            if (e.Result is FrameRenderedResult frameRenderedResult)
            {
                IntPtr ip = frameRenderedResult.Frame.GetHbitmap();
                BitmapSource bs = null;
                try
                {
                    bs = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(ip,
                        IntPtr.Zero, 
                        Int32Rect.Empty,
                        BitmapSizeOptions.FromEmptyOptions());
                }
                finally
                {
                    DeleteObject(ip);
                }

                Frame = bs;
            }
            else if (e.Result is FrameRenderingFailure frameRenderingFailure)
            {
                RenderingError = BuildError(Resources.Strings.Error_RenderingFailed, frameRenderingFailure.Exception);
            }
        }

        private void UpdateFrame()
        {
            if (frameRendererWorker != null && frameRendererWorker.IsBusy)
            {
                frameRendererWorker.CancelAsync();
                frameRendererWorker = null;
            }

            frameRendererWorker = new FrameRendererWorker();
            frameRendererWorker.RunWorkerCompleted += FrameRendered;
            frameRendererWorker.RunWorkerAsync(new FrameRenderInput(movie, frameIndex));
        }

        private List<string> CollectRootLevelSuggestions()
        {
            return new List<string> { nameof(Movie) };
        }

        private List<string> CollectChildElementSuggestionsFor(string name)
        {
            // Find type for name

            var movieType = typeof(Movie);
            var movieAssembly = movieType.Assembly;
            var elementsNamespace = movieType.Namespace;

            var result = new List<string>();

            var type = movieAssembly.GetType($"{elementsNamespace}.{name}");
            if (type != null)
            {
                // Collect properties

                StaticInitializeRecursively(type);
                var properties = ManagedProperty.FindAllByType(type, true);
                foreach (var property in properties.OrderBy(p => p.Name))
                {
                    result.Add($"{type.Name}.{property.Name}");
                }
            }

            // Collect all types from the elements namespace

            var types = movieAssembly.GetTypes().Where(t => t.Namespace == elementsNamespace && t.IsPublic);
            foreach (var elementType in types.OrderBy(t => t.Name))
            {
                result.Add($"{elementType.Name}");
            }

            return result;
        }

        private static void StaticInitializeRecursively(Type type)
        {
            do
            {
                System.Runtime.CompilerServices.RuntimeHelpers.RunClassConstructor(type.TypeHandle);
                type = type.BaseType;
            }
            while (type != typeof(ManagedObject) && type != typeof(object));
        }

        private List<string> CollectPropertySuggestionsFor(string name)
        {
            // Find type for name

            var movieType = typeof(Movie);
            var movieAssembly = movieType.Assembly;
            var elementsNamespace = movieType.Namespace;

            var type = movieAssembly.GetType($"{elementsNamespace}.{name}");
            if (type == null)
                return new List<string>();

            // Collect properties

            var result = new List<string>();

            StaticInitializeRecursively(type);
            var properties = ManagedProperty.FindAllByType(type, true);
            foreach (var property in properties.OrderBy(p => p.Name))
            {
                result.Add($"{property.Name}");
            }

            return result;
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

            updateMovieWorker = new UpdateMovieWorker();
            updateMovieWorker.RunWorkerCompleted += UpdateMovieFinished;
            updateMovieWorker.RunWorkerAsync(new UpdateMovieInput(text));
        }

        public List<string> GetCompletionList(int selectionStart)
        {
            string text = document.Text;

            var root = Parser.ParseText(text);
            var currentNode = root.FindNode(selectionStart, includeTrivia: true);

            while (currentNode != null)
            {
                if (currentNode is XmlElementStartTagSyntax xmlStart)
                {
                    return CollectPropertySuggestionsFor(xmlStart.Name);
                }
                else if (currentNode is XmlElementSyntax xmlElement)
                {
                    return CollectChildElementSuggestionsFor(xmlElement.Name);
                }

                currentNode = currentNode.Parent;
            }

            if (currentNode == null)
                return CollectRootLevelSuggestions();

            return new List<string>();
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

        public int MinFrame
        {
            get => minFrame;
            set => Set(ref minFrame, () => MinFrame, value);
        }

        public int FrameIndex
        {
            get => frameIndex;
            set => Set(ref frameIndex, () => FrameIndex, value, HandleFrameIndexChanged, true);
        }

        public int MaxFrame
        {
            get => maxFrame;
            set => Set(ref maxFrame, () => MaxFrame, value);
        }

        public BitmapSource Frame
        {
            get => frame;
            set => Set(ref frame, () => Frame, value);
        }

        public string ParsingError
        {
            get => parsingError;
            set => Set(ref parsingError, () => ParsingError, value, HandleErrorChanged);
        }

        public string RenderingError
        {
            get => renderingError;
            set => Set(ref renderingError, () => RenderingError, value, HandleErrorChanged);
        }

        public string Error => BuildErrors(ParsingError, RenderingError);

        public bool ShowError
        {
            get => !String.IsNullOrEmpty(ParsingError) || !String.IsNullOrEmpty(RenderingError);
        }
    }
}
