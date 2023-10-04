using Animator.Designer.BusinessLogic.ViewModels.Base;
using Animator.Designer.BusinessLogic.ViewModels.Wrappers;
using Animator.Designer.BusinessLogic.ViewModels.Wrappers.Objects;
using Animator.Engine.Base.Exceptions;
using Animator.Engine.Elements;
using Animator.Engine.Exceptions;
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
using System.Windows;
using System.Windows.Media.Imaging;
using System.Xml;
using System.Xml.Linq;

namespace Animator.Designer.BusinessLogic.ViewModels.Main
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

        private readonly ObjectViewModel[] displayItems;
        private ObjectViewModel selectedElement;

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

        private void HandleFrameIndexChanged()
        {
            UpdateFrame();
        }

        private void HandleErrorChanged()
        {
            OnPropertyChanged(nameof(Error));
            OnPropertyChanged(nameof(ShowError));
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

                RenderingStatus = String.Format(Resources.Windows.MainWindow.Strings.Message_FrameRendered, frameRenderedResult.Duration);
            }
            else if (e.Result is FrameRenderingFailure frameRenderingFailure)
            {
                RenderingError = BuildError(Resources.Windows.MainWindow.Strings.Error_RenderingFailed, frameRenderingFailure.Exception);
                RenderingStatus = Resources.Windows.MainWindow.Strings.Message_FrameFailedToRender;
            }
        }

        private void UpdateFrame()
        {
            if (frameRendererWorker != null && frameRendererWorker.IsBusy)
            {
                frameRendererWorker.CancelAsync();
                frameRendererWorker = null;
            }

            RenderingStatus = Resources.Windows.MainWindow.Strings.Message_RenderingFrame;

            frameRendererWorker = new FrameRendererWorker();
            frameRendererWorker.RunWorkerCompleted += FrameRendered;
            frameRendererWorker.RunWorkerAsync(new FrameRenderInput(movie, frameIndex));
        }

        private void HandleMovieChanged(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        // Public methods -----------------------------------------------------

        public DocumentViewModel(ObjectViewModel rootNode, WrapperContext wrapperContext, string filename = "Animation.xml", bool filenameVirtual = true)
        {
            RootNode = rootNode;
            WrapperContext = wrapperContext;

            displayItems = new[] { rootNode };

            Filename = filename;
            FilenameVirtual = filenameVirtual;

            WrapperContext.MovieChanged += HandleMovieChanged;
        }

        public void UpdateMovie()
        {
            if (updateMovieWorker != null && updateMovieWorker.IsBusy)
            {
                updateMovieWorker.CancelAsync();
                updateMovieWorker = null;
            }

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

        public TStream Save<TStream>(Func<TStream> streamFactory)
            where TStream : Stream
        {
            XmlDocument xmlDocument = new XmlDocument();

            XmlElement rootNode = RootNode.Serialize(xmlDocument);
            WrapperContext.ApplyNamespaces(xmlDocument, rootNode);
            xmlDocument.AppendChild(rootNode);

            TStream stream = streamFactory();
            XmlTextWriter writer = new XmlTextWriter(stream, Encoding.Unicode);
            writer.Formatting = Formatting.Indented;

            xmlDocument.Save(writer);

            return stream;
        }

        public string Filename { get; set; }
        public bool FilenameVirtual { get; set; }
        public ObjectViewModel RootNode { get; }
        public WrapperContext WrapperContext { get; }

        public IEnumerable<ObjectViewModel> DisplayItems => displayItems;
        
        public ObjectViewModel SelectedElement 
        {
            get => selectedElement;
            set
            {
                Set(ref selectedElement, value);
            }
        }

        public int MinFrame
        {
            get => minFrame;
            set => Set(ref minFrame, value);
        }

        public int FrameIndex
        {
            get => frameIndex;
            set => Set(ref frameIndex, value, nameof(FrameIndex), HandleFrameIndexChanged, true);
        }

        public int MaxFrame
        {
            get => maxFrame;
            set => Set(ref maxFrame, value);
        }

        public BitmapSource Frame
        {
            get => frame;
            set => Set(ref frame, value);
        }

        public string ParsingError
        {
            get => parsingError;
            set => Set(ref parsingError, value, nameof(ParsingError), HandleErrorChanged);
        }

        public string RenderingError
        {
            get => renderingError;
            set => Set(ref renderingError, value, nameof(RenderingError), HandleErrorChanged);
        }

        public string RenderingStatus
        {
            get => renderingStatus;
            set => Set(ref renderingStatus, value);
        }

        public string Error => BuildErrors(ParsingError, RenderingError);

        public bool ShowError
        {
            get => !String.IsNullOrEmpty(ParsingError) || !String.IsNullOrEmpty(RenderingError);
        }
    }
}
