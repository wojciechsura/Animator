using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
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
using Animator.Engine.Base;
using Animator.Engine.Base.Exceptions;
using Animator.Engine.Base.Persistence;
using Animator.Engine.Elements;
using Animator.Engine.Elements.Persistence;
using Animator.Engine.Exceptions;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Document;
using Microsoft.Language.Xml;
using Spooksoft.VisualStateManager.Commands;

namespace Animator.Editor.BusinessLogic.ViewModels.Document
{
    public class DocumentViewModel : BaseViewModel
    {
        // Private classes ----------------------------------------------------

        private class UpdateMovieInput
        {
            public UpdateMovieInput(string movieXml, string path)
            {
                MovieXml = movieXml;
                Path = path;
            }

            public string MovieXml { get; }
            public string Path { get; }
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

                    movie = movieSerializer.Deserialize(document, System.IO.Path.GetDirectoryName(input.Path));
                    movie.Path = input.Path;
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
            public FrameRenderedResult(Bitmap frame, TimeSpan time)
            {
                Frame = frame;
                Duration = time;
            }

            public Bitmap Frame { get; }
            public TimeSpan Duration { get; }
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
                    Stopwatch stopwatch = Stopwatch.StartNew();

                    var framesPerSecond = input.Movie.Config.FramesPerSecond;
                    TimeSpan time = TimeSpan.FromSeconds(1 / framesPerSecond * input.FrameIndex);
                    Bitmap result = RenderFrameAt(input.Movie, time);

                    stopwatch.Stop();

                    e.Result = new FrameRenderedResult(result, stopwatch.Elapsed);
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
        private string renderingStatus;

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

                RenderingStatus = String.Format(Resources.Strings.Message_FrameRendered, frameRenderedResult.Duration);
            }
            else if (e.Result is FrameRenderingFailure frameRenderingFailure)
            {
                RenderingError = BuildError(Resources.Strings.Error_RenderingFailed, frameRenderingFailure.Exception);
                RenderingStatus = Resources.Strings.Message_FrameFailedToRender;
            }
        }

        private void UpdateFrame()
        {
            if (frameRendererWorker != null && frameRendererWorker.IsBusy)
            {
                frameRendererWorker.CancelAsync();
                frameRendererWorker = null;
            }

            RenderingStatus = Resources.Strings.Message_RenderingFrame;

            frameRendererWorker = new FrameRendererWorker();
            frameRendererWorker.RunWorkerCompleted += FrameRendered;
            frameRendererWorker.RunWorkerAsync(new FrameRenderInput(movie, frameIndex));
        }

        private CompletionInfo TagToCompletionInfo(string tag, bool autoClosed = false)
        {
            if (autoClosed)
                return new CompletionInfo(tag, $"<{tag} />", tag.Length + 2);
            else
                return new CompletionInfo(tag, $"<{tag}></{tag}>", tag.Length + 2);
        }

        private CompletionInfo AttributeToCompletionInfo(string attr)
        {
            return new CompletionInfo(attr, $"{attr}=\"\"", attr.Length + 2);
        }

        private CompletionInfo ValueToCompletionInfo(string value)
        {
            return new CompletionInfo(value, value, value.Length);
        }

        private List<CompletionInfo> CollectRootLevelSuggestions()
        {
            return new List<CompletionInfo> { TagToCompletionInfo(nameof(Movie), false) };
        }

        private List<CompletionInfo> CollectPropertySuggestions(Type type, string propertyName)
        {
            var movieType = typeof(Movie);
            var movieAssembly = movieType.Assembly;
            var elementsNamespace = movieType.Namespace;

            var property = ManagedProperty.FindByTypeAndName(type, propertyName);
            if (property == null)
                return null;

            if (property is ManagedCollectionProperty collectionProperty)
            {
                var propertyType = collectionProperty.Type;
                if (!propertyType.IsGenericType || propertyType.GetGenericTypeDefinition() != typeof(ManagedCollection<>))
                    return null;

                var genericType = propertyType.GenericTypeArguments[0];

                var result = new List<CompletionInfo>();

                foreach (var derivedType in movieAssembly.GetTypes()
                    .Where(t => t.IsPublic && !t.IsAbstract && t.Namespace == elementsNamespace && genericType.IsAssignableFrom(t)))
                {
                    var contentPropertyAttributes = derivedType.GetCustomAttributes(typeof(ContentPropertyAttribute), false);

                    result.Add(TagToCompletionInfo(derivedType.Name, !contentPropertyAttributes.Any()));
                }

                return result;
            }
            else if (property is ManagedReferenceProperty refProperty)
            {
                var propertyType = refProperty.Type;

                var result = new List<CompletionInfo>();

                foreach (var derivedType in movieAssembly.GetTypes()
                    .Where(t => t.IsPublic && !t.IsAbstract && t.Namespace == elementsNamespace && propertyType.IsAssignableFrom(t)))
                {
                    var contentPropertyAttributes = derivedType.GetCustomAttributes(typeof(ContentPropertyAttribute), false);

                    result.Add(TagToCompletionInfo(derivedType.Name, !contentPropertyAttributes.Any()));
                }

                return result;
            }
            else
            {
                return null;
            }
        }

        private List<CompletionInfo> CollectChildSuggestionsFor(string name)
        {
            // Find type for name
            var movieType = typeof(Movie);
            var movieAssembly = movieType.Assembly;
            var elementsNamespace = movieType.Namespace;

            if (name == "x:Macros")
            {
                var result = new List<CompletionInfo>();

                foreach (var type in movieAssembly.GetTypes()
                    .Where(t => t.IsPublic && !t.IsAbstract && t.Namespace == elementsNamespace))
                {
                    var contentPropertyAttributes = type.GetCustomAttributes(typeof(ContentPropertyAttribute), false);
                    result.Add(TagToCompletionInfo(type.Name, !contentPropertyAttributes.Any()));
                }

                return result;
            }
            else if (Regex.IsMatch(name, @"[^\.]+\.[^\.]+"))
            {
                string[] splitted = name.Split('.');
                string typeName = splitted[0];
                string propName = splitted[1];

                var type = movieAssembly.GetType($"{elementsNamespace}.{typeName}");
                if (type == null)
                    return null;

                return CollectPropertySuggestions(type, propName);
            }
            else
            {
                var type = movieAssembly.GetType($"{elementsNamespace}.{name}");
                if (type != null)
                {
                    List<CompletionInfo> result = new();
                    foreach (var property in ManagedProperty.FindAllByType(type, true))
                        result.Add(TagToCompletionInfo($"{type.Name}.{property.Name}", false));

                    var contentPropertyAttribute = (ContentPropertyAttribute[])type.GetCustomAttributes(typeof(ContentPropertyAttribute), false);
                    if (contentPropertyAttribute.Length == 1)
                    {
                        var defaultPropertyName = contentPropertyAttribute[0].PropertyName;
                        var collectionSuggestions = CollectPropertySuggestions(type, defaultPropertyName);
                        if (collectionSuggestions != null)
                            result.AddRange(collectionSuggestions);
                    }

                    return result;
                }
            }

            return null;
        }

        private List<CompletionInfo> CollectPropertySuggestionsFor(string name)
        {
            // Find type for name

            var movieType = typeof(Movie);
            var movieAssembly = movieType.Assembly;
            var elementsNamespace = movieType.Namespace;

            var type = movieAssembly.GetType($"{elementsNamespace}.{name}");
            if (type == null)
                return new List<CompletionInfo>();

            // Collect properties

            var result = new List<CompletionInfo>();

            var properties = ManagedProperty.FindAllByType(type, true);
            foreach (var property in properties.OrderBy(p => p.Name))
            {
                result.Add(AttributeToCompletionInfo($"{property.Name}"));
            }

            return result;
        }

        private List<CompletionInfo> CollectPropertyValueSuggestionsFor(string name, string attribute)
        {
            // Find type for name

            var movieType = typeof(Movie);
            var movieAssembly = movieType.Assembly;
            var elementsNamespace = movieType.Namespace;

            var type = movieAssembly.GetType($"{elementsNamespace}.{name}");
            if (type == null)
                return new List<CompletionInfo>();

            // Find property

            var result = new List<CompletionInfo>();

            var prop = ManagedProperty.FindByTypeAndName(type, attribute, true);

            if (prop != null && prop.Type.IsEnum)
                Enum.GetValues(prop.Type)
                    .OfType<object>()
                    .Select(o => o.ToString())
                    .OrderBy(o => o)
                    .ToList()
                    .ForEach(v => result.Add(ValueToCompletionInfo(v)));

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
            updateMovieWorker.RunWorkerAsync(new UpdateMovieInput(text, FilenameVirtual ? string.Empty : FileName));
        }

        public List<CompletionInfo> GetCompletionList(int selectionStart)
        {
            string text = document.Text;

            var root = Parser.ParseText(text);
            var currentNode = root.FindNode(selectionStart, includeTrivia: true);

            /* DEBUG */
            var node2 = currentNode;
            do
            {
                Debug.Write($"{node2.GetType().Name} -> ");
                node2 = node2.Parent;
            }
            while (node2 != null);
            Debug.WriteLine(string.Empty);
            /* DEBUG */

            string attribute = null;
            bool insideAttribute = false;

            while (currentNode != null)
            {
                if (currentNode is XmlElementStartTagSyntax xmlStart)
                {
                    if (attribute != null)
                        return CollectPropertyValueSuggestionsFor(xmlStart.Name, attribute);
                    else
                        return CollectPropertySuggestionsFor(xmlStart.Name);
                }
                else if (currentNode is XmlEmptyElementSyntax xmlEmpty)
                {
                    if (attribute != null)
                        return CollectPropertyValueSuggestionsFor(xmlEmpty.Name, attribute);
                    else
                        return CollectPropertySuggestionsFor(xmlEmpty.Name);
                }
                else if (currentNode is XmlElementSyntax xmlElement)
                {
                    return CollectChildSuggestionsFor(xmlElement.Name);
                }
                else if (currentNode is XmlStringSyntax)
                {
                    insideAttribute = true;
                }
                else if (currentNode is XmlAttributeSyntax xmlAttribute)
                {
                    if (insideAttribute)
                        attribute = xmlAttribute.Name;
                }

                currentNode = currentNode.Parent;
            }

            if (currentNode == null)
                return CollectRootLevelSuggestions();

            return new List<CompletionInfo>();
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

        public string RenderingStatus 
        {
            get => renderingStatus;
            set => Set(ref renderingStatus, () => RenderingStatus, value);
        }

        public string Error => BuildErrors(ParsingError, RenderingError);

        public bool ShowError
        {
            get => !String.IsNullOrEmpty(ParsingError) || !String.IsNullOrEmpty(RenderingError);
        }
    }
}
