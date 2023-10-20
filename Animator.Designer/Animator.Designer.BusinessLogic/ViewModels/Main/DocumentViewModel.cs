using Animator.Designer.BusinessLogic.Services.Dialogs;
using Animator.Designer.BusinessLogic.ViewModels.Base;
using Animator.Designer.BusinessLogic.ViewModels.Wrappers;
using Animator.Designer.BusinessLogic.ViewModels.Wrappers.Objects;
using Animator.Engine.Base.Exceptions;
using Animator.Engine.Elements;
using Animator.Engine.Exceptions;
using Spooksoft.VisualStateManager.Commands;
using Spooksoft.VisualStateManager.Conditions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Xml;
using System.Xml.Linq;

namespace Animator.Designer.BusinessLogic.ViewModels.Main
{
    public class DocumentViewModel : BaseViewModel, IDisposable
    {
        // Private classes ----------------------------------------------------

        private sealed record class FrameRenderedResult(Bitmap Frame, TimeSpan Duration);
        
        private sealed class FrameRendererWorker : BackgroundWorker
        {
            private Bitmap RenderFrameAt(Movie movie, int scene, TimeSpan sceneTimeOffset)
            {
                movie.Scenes[scene].ApplyAnimation((float)sceneTimeOffset.TotalMilliseconds);

                var result = new Bitmap(movie.Config.Width, movie.Config.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                movie.Scenes[scene].Render(result);

                return result;
            }

            protected override void OnDoWork(DoWorkEventArgs e)
            {
                var input = e.Argument as FrameRenderInput;

                try
                {
                    Stopwatch stopwatch = Stopwatch.StartNew();

                    Bitmap result = RenderFrameAt(input.Movie, input.SceneIndex, input.SceneTimeOffset);

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

        private sealed class FrameRenderingFailure
        {
            public FrameRenderingFailure(Exception exception)
            {
                Exception = exception;
            }

            public Exception Exception { get; }
        }

        private sealed record class FrameRenderInput(Movie Movie, int SceneIndex, TimeSpan SceneTimeOffset);        

        private sealed class MovieUpdatedResult
        {
            public MovieUpdatedResult(Movie animation)
            {
                Movie = animation;
            }

            public Movie Movie { get; }
        }

        private sealed class MovieUpdateFailed
        {
            public MovieUpdateFailed(Exception exception)
            {
                Exception = exception;
            }

            public Exception Exception { get; }
        }

        private sealed class UpdateMovieInput
        {
            public UpdateMovieInput(string movieXml, string path)
            {
                MovieXml = movieXml;
                Path = path;
            }

            public string MovieXml { get; }
            public string Path { get; }
        }

        private sealed class UpdateMovieWorker : BackgroundWorker
        {
            private static readonly Animator.Engine.Elements.Persistence.MovieSerializer movieSerializer = new();

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

        // Private fields -----------------------------------------------------

        private readonly ObjectViewModel[] displayItems;
        private readonly IDialogService dialogService;
        private BitmapSource frame;
        private int frameIndex;
        private FrameRendererWorker frameRendererWorker;
        private int maxFrame;
        private int minFrame;
        private Movie movie;
        private string parsingError;
        private string renderingError;
        private string renderingStatus;
        private ObjectViewModel selectedElement;
        private UpdateMovieWorker updateMovieWorker;
        private string movieTime;
        private string sceneTime;
        private string sceneIndex;

        private bool changed;

        // Private methods ----------------------------------------------------

        [DllImport("gdi32")]
        private static extern int DeleteObject(IntPtr o);

        private string BuildError(string message, Exception exception)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(message);
            sb.AppendLine(Resources.Windows.MainWindow.Strings.Error_Reason);
            do
            {
                if (exception is ParseException parseException)
                    sb.AppendLine($"    {parseException.Message}");
                else if (exception is SerializerException serializerException)
                    sb.AppendLine($"    {serializerException.Message} {Resources.Windows.MainWindow.Strings.Error_At} {serializerException.XPath}");
                else if (exception is AnimationException animationException)
                    sb.AppendLine($"    {animationException.Message} {Resources.Windows.MainWindow.Strings.Error_At} {animationException.Path}");
                else
                    sb.AppendLine($"    {exception.Message}");

                exception = exception.InnerException;
                if (exception != null)
                    sb.AppendLine(Resources.Windows.MainWindow.Strings.Error_Reason);
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

        private void DoEditMacroProperties()
        {
            dialogService.ShowMacroPropertyEditor(selectedElement as MacroViewModel);
        }

        private void FrameRendered(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
                return;

            RenderingError = null;

            if (e.Result is FrameRenderedResult frameRenderedResult)
            {
                // Render frame

                IntPtr ip = frameRenderedResult.Frame.GetHbitmap();
                BitmapSource bs = null;
                try
                {
                    bs = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(ip,
                        IntPtr.Zero,
                        System.Windows.Int32Rect.Empty,
                        BitmapSizeOptions.FromEmptyOptions());
                }
                finally
                {
                    DeleteObject(ip);
                }

                Frame = bs;

                RenderingStatus = String.Format(Resources.Windows.MainWindow.Strings.Message_FrameRendered, frameRenderedResult.Duration);
            }
            else if (e.Result is FrameRenderingFailure frameRenderingFailure)
            {
                RenderingError = BuildError(Resources.Windows.MainWindow.Strings.Error_RenderingFailed, frameRenderingFailure.Exception);
                RenderingStatus = Resources.Windows.MainWindow.Strings.Message_FrameFailedToRender;
            }
        }

        private void HandleErrorChanged()
        {
            OnPropertyChanged(nameof(Error));
            OnPropertyChanged(nameof(ShowError));
        }

        private void HandleFrameIndexChanged()
        {
            UpdateFrame();
        }

        private void UpdateFrame()
        {
            if (frameRendererWorker != null && frameRendererWorker.IsBusy)
            {
                frameRendererWorker.CancelAsync();
                frameRendererWorker = null;
            }

            RenderingStatus = Resources.Windows.MainWindow.Strings.Message_RenderingFrame;

            // Calculate times

            var framesPerSecond = movie.Config.FramesPerSecond;
            TimeSpan movieTime = TimeSpan.FromSeconds(1 / framesPerSecond * frameIndex);

            if (movie.Scenes.Count == 0)
                throw new InvalidOperationException("No scenes to render!");

            TimeSpan summedTime = TimeSpan.FromSeconds(0);

            int scene = 0;
            while (scene < movie.Scenes.Count && summedTime + movie.Scenes[scene].Duration < movieTime)
            {
                summedTime += movie.Scenes[scene].Duration;
                scene++;
            }

            if (scene >= movie.Scenes.Count)
                throw new InvalidOperationException("Given time exceedes whole movie time!");

            TimeSpan sceneTime = movieTime - summedTime;

            MovieTime = movieTime.ToString("hh\\:mm\\:ss\\.ff");
            SceneTime = sceneTime.ToString("hh\\:mm\\:ss\\.ff");
            SceneIndex = scene.ToString();

            // Run worker

            frameRendererWorker = new FrameRendererWorker();
            frameRendererWorker.RunWorkerCompleted += FrameRendered;
            frameRendererWorker.RunWorkerAsync(new FrameRenderInput(movie, scene, sceneTime));
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
                ParsingError = BuildError(Resources.Windows.MainWindow.Strings.Error_ParsingFailed, movieUpdateFailed.Exception);
            }
        }

        // Public methods -----------------------------------------------------

        public DocumentViewModel(IDialogService dialogService,
            ObjectViewModel rootNode, 
            WrapperContext wrapperContext, 
            string filename = "Animation.xml", 
            bool filenameVirtual = true)
        {
            this.dialogService = dialogService;
            this.changed = false;

            RootNode = rootNode;
            WrapperContext = wrapperContext;
            wrapperContext.MovieChanged += (s, e) => Changed = true;

            displayItems = new[] { rootNode };

            Filename = filename;
            FilenameVirtual = filenameVirtual;
            var macroSelectedCondition = Condition.Lambda(this, vm => vm.SelectedElement is MacroViewModel, false);

            EditMacroProperties = new AppCommand(obj => DoEditMacroProperties(), macroSelectedCondition);
        }

        public void Dispose()
        {
            if (frameRendererWorker != null && frameRendererWorker.IsBusy)
                frameRendererWorker.CancelAsync();
            frameRendererWorker = null;

            if (updateMovieWorker != null && updateMovieWorker.IsBusy)
                updateMovieWorker.CancelAsync();
            updateMovieWorker = null;            
        }

        public TStream Save<TStream>(Func<TStream> streamFactory)
            where TStream : Stream
        {
            XmlDocument xmlDocument = new XmlDocument();

            XmlElement rootNode = RootNode.Serialize(xmlDocument);
            WrapperContext.ApplyNamespaces(xmlDocument, rootNode);
            xmlDocument.AppendChild(rootNode);

            TStream stream = streamFactory();
            XmlTextWriter writer = new XmlTextWriter(stream, Encoding.UTF8);
            writer.Formatting = Formatting.Indented;

            xmlDocument.Save(writer);

            return stream;
        }

        public void UpdateMovie()
        {
            if (updateMovieWorker != null && updateMovieWorker.IsBusy)
            {
                updateMovieWorker.CancelAsync();
                updateMovieWorker = null;
            }

            MovieTime = string.Empty;
            SceneTime = string.Empty;
            SceneIndex = string.Empty;

            try
            {
                MemoryStream ms = Save(() => new MemoryStream());
                ms.Seek(0, SeekOrigin.Begin);

                var reader = new StreamReader(ms);
                var text = reader.ReadToEnd();

                updateMovieWorker = new UpdateMovieWorker();
                updateMovieWorker.RunWorkerCompleted += UpdateMovieFinished;
                updateMovieWorker.RunWorkerAsync(new UpdateMovieInput(text, FilenameVirtual ? string.Empty : Filename));
            }
            catch (Exception e)
            {
                RenderingError = BuildError(Resources.Windows.MainWindow.Strings.Error_SerializationFailed, e);
            }            
        }

        public void NotifyAvailableNamespacesChanged()
        {
            RootNode.NotifyAvailableTypesChanged();
        }

        // Public properties --------------------------------------------------

        public bool Changed
        {
            get => changed;
            set => Set(ref changed, value);
        }

        public IEnumerable<ObjectViewModel> DisplayItems => displayItems;

        public ICommand EditMacroProperties { get; }

        public string Error => BuildErrors(ParsingError, RenderingError);

        public string Filename { get; set; }

        public bool FilenameVirtual { get; set; }

        public BitmapSource Frame
        {
            get => frame;
            set => Set(ref frame, value);
        }

        public string MovieTime
        {
            get => movieTime;
            set => Set(ref movieTime, value);
        }

        public string SceneTime
        {
            get => sceneTime;
            set => Set(ref sceneTime, value);
        }

        public string SceneIndex
        {
            get => sceneIndex;
            set => Set(ref sceneIndex, value);
        }

        public int FrameIndex
        {
            get => frameIndex;
            set => Set(ref frameIndex, value, changeHandler: HandleFrameIndexChanged, force: true);
        }

        public int MaxFrame
        {
            get => maxFrame;
            set => Set(ref maxFrame, value);
        }

        public int MinFrame
        {
            get => minFrame;
            set => Set(ref minFrame, value);
        }

        public string ParsingError
        {
            get => parsingError;
            set => Set(ref parsingError, value, changeHandler: HandleErrorChanged);
        }

        public string RenderingError
        {
            get => renderingError;
            set => Set(ref renderingError, value, changeHandler: HandleErrorChanged);
        }

        public string RenderingStatus
        {
            get => renderingStatus;
            set => Set(ref renderingStatus, value);
        }

        public ObjectViewModel RootNode { get; }

        public ObjectViewModel SelectedElement
        {
            get => selectedElement;
            set
            {
                Set(ref selectedElement, value);
            }
        }

        public bool ShowError
        {
            get => !String.IsNullOrEmpty(ParsingError) || !String.IsNullOrEmpty(RenderingError);
        }

        public WrapperContext WrapperContext { get; }
    }
}
