using Animator.Engine.Animation;
using Animator.Engine.Animation.Maths;
using Animator.Engine.Base.Exceptions;
using Animator.Engine.Elements;
using Animator.Engine.Elements.Persistence;
using Animator.Engine.Exceptions;
using Animator.Options;
using CommandLine;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;

namespace Animator
{
    class Program
    {
        private readonly static object consoleLock = new object();

        private static void Write(string text = "", ConsoleColor color = ConsoleColor.Gray)
        {
            lock (consoleLock)
            {
                Console.ForegroundColor = color;
                Console.Write(text);
            }
        }

        private static void WriteLine(string text = "", ConsoleColor color = ConsoleColor.Gray)
        {
            lock (consoleLock)
            {
                Console.ForegroundColor = color;
                Console.WriteLine(text);
            }
        }

        private static void Write(string text1, string text2, ConsoleColor color1, ConsoleColor color2 = ConsoleColor.Gray)
        {
            lock (consoleLock)
            {
                Console.ForegroundColor = color1;
                Console.Write(text1);
                Console.ForegroundColor = color2;
                Console.Write(text2);
            }
        }


        private static void WriteLine(string text1, string text2, ConsoleColor color1, ConsoleColor color2 = ConsoleColor.Gray)
        {
            lock (consoleLock)
            {
                Console.ForegroundColor = color1;
                Console.Write(text1);
                Console.ForegroundColor = color2;
                Console.WriteLine(text2);
            }
        }

        private static Movie LoadAnimation(string path)
        {
            XmlDocument document = new();
            document.Load(path);

            MovieSerializer animationSerializer = new MovieSerializer();
            Movie animation = animationSerializer.Deserialize(document);
            animation.Path = path;

            return animation;
        }

        private static Movie[] LoadAnimations(string path, int copies)
        {
            var result = new Movie[copies];

            for (int i = 0; i < copies; i++)
                result[i] = LoadAnimation(path);

            return result;
        }

        private static void DisplayError(Exception e)
        {
            WriteLine("--- Reason: ---");
            WriteLine();

            while (e != null)
            {
                if (e is SerializerException serializerException)
                {
                    WriteLine(serializerException.Message);
                    WriteLine($"(on {serializerException.XPath})");
                }
                else if (e is AnimationException animationException)
                {
                    WriteLine(animationException.Message);
                    WriteLine($"(on {animationException.Path})");
                }
                else
                    WriteLine(e.Message);

                if (e.InnerException != null)
                {
                    WriteLine();
                    WriteLine("--- Because: ---");
                    WriteLine();
                }

                e = e.InnerException;
            }
        }

        private static bool IsSuccessful(Action action, Action writeMessage)
        {
            using (new DisposableStopwatch(writeMessage))
                return IsSuccessful(action);
        }

        private static bool IsSuccessful(Action action)
        {
            try
            {
                action();
                return true;
            }
            catch (Exception e)
            {
                WriteLine("Erorr: ", "Failed to generate animation.", ConsoleColor.Red);
                DisplayError(e);
                return false;
            }
        }

        private static Bitmap RenderAnimationAt(Movie animation, TimeSpan time, Bitmap previousFrame, string outFile)
        {
            // Determine scene

            TimeSpan summedTime = TimeSpan.FromSeconds(0);

            int i = 0;
            while (i < animation.Scenes.Count && summedTime + animation.Scenes[i].Duration < time)
            {
                summedTime += animation.Scenes[i].Duration;
                i++;
            }

            if (i >= animation.Scenes.Count)
                throw new InvalidOperationException("Given time exceedes whole animation time!");

            TimeSpan sceneTimeOffset = time - summedTime;

            bool changed = animation.Scenes[i].ApplyAnimation((float)sceneTimeOffset.TotalMilliseconds);

            if (!changed && previousFrame != null && !animation.Scenes[i].AlwaysRender)
            {
                previousFrame.Save(outFile);
                return previousFrame;
            }
            else
            {
                var bitmap = new Bitmap(animation.Config.Width, animation.Config.Height, PixelFormat.Format32bppArgb);
                animation.Scenes[i].Render(bitmap);
                bitmap.Save(outFile);

                return bitmap;
            }            
        }

        private static void RenderFrame(RenderFrameOptions options)
        {
            Movie animation = null;


            if (!IsSuccessful(() => { animation = LoadAnimation(options.Source); }, () => Write("Loaded animation")))
                return;

            var framesPerSecond = animation.Config.FramesPerSecond;

            if (options.FrameIndex != null)
            {
                TimeSpan time = TimeSpan.FromSeconds(1 / framesPerSecond * options.FrameIndex.Value);

                if (!IsSuccessful(() => RenderAnimationAt(animation, time, null, options.OutFile), () => Write("Rendered single frame")))
                    return;
                
            }
            else if (options.TimeOffset != null)
            {
                if (!IsSuccessful(() => RenderAnimationAt(animation, options.TimeOffset.Value, null, options.OutFile), () => Write("Rendered single frame")))
                    return;
            }
        }

        private static void Render(RenderOptions options)
        {
            Movie animation = null;

            if (!IsSuccessful(() => { animation = LoadAnimation(options.Source); }))
                return;

            var framesPerSecond = animation.Config.FramesPerSecond;
            var totalAnimationTime = animation.Scenes.Sum(s => s.Duration.TotalMilliseconds);
            var totalFrames = (int)(totalAnimationTime / 1000.0f * framesPerSecond);
            Bitmap previousFrame = null;

            for (int frame = 0; frame < totalFrames; frame++)
            {
                var time = TimeSpan.FromSeconds(1 / framesPerSecond * frame);

                string fileName = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(options.OutFile),
                    $"{System.IO.Path.GetFileNameWithoutExtension(options.OutFile)}{frame}{System.IO.Path.GetExtension(options.OutFile)}");

                if (!IsSuccessful(() =>
                    {
                        previousFrame = RenderAnimationAt(animation, time, previousFrame, fileName);
                    }, () => 
                    {
                        Write("Rendered frame ", $"{frame + 1}", ConsoleColor.Gray, ConsoleColor.White);
                        Write(" from ", $"{totalFrames}", ConsoleColor.Gray, ConsoleColor.White);
                    }))
                {
                    return;
                }
            }
        }

        private static void RenderParallel(RenderParallelOptions options)
        {
            Movie[] animations = null;

            if (!IsSuccessful(() => { animations = LoadAnimations(options.Source, options.Threads); }))
                return;

            var framesPerSecond = animations[0].Config.FramesPerSecond;
            var totalAnimationTime = animations[0].Scenes.Sum(s => s.Duration.TotalMilliseconds);
            var totalFrames = (int)(totalAnimationTime / 1000.0f * framesPerSecond);

            object sharedDataLock = new();

            // Frame ranges

            List<(int Start, int End)> frameRanges;

            if (totalFrames < options.Threads)
            {
                frameRanges = new List<(int Start, int End)> { (0, totalFrames - 1) };
            }
            else
            {
                frameRanges = Enumerable.Range(0, options.Threads)
                    .Select(i => ( Start: Math.Min(totalFrames - 1, i * totalFrames / options.Threads), 
                        End: Math.Min(totalFrames, (i + 1) * totalFrames / options.Threads - 1)))
                    .ToList();
            }

            WriteLine("Render plan:", ConsoleColor.White);

            for (int i = 0; i < frameRanges.Count; i++)
                WriteLine($"{i + 1}. ", $"({frameRanges[i].Start} - {frameRanges[i].End})", ConsoleColor.White);

            // Movie object repository

            Stack<Movie> availableMovies = new();
            foreach (var movie in animations)
                availableMovies.Push(movie);

            ParallelOptions parallelOptions = new ParallelOptions
            {
                MaxDegreeOfParallelism = options.Threads
            };

            Parallel.For(0, frameRanges.Count, parallelOptions, (index, state) =>
                {
                    Movie animation;
                    (int Start, int End) frameRange;                    

                    lock (sharedDataLock)
                    {
                        animation = availableMovies.Pop();
                        frameRange = frameRanges[index];
                    }

                    Bitmap previousFrame = null;

                    for (int frame = frameRange.Start; frame < frameRange.End; frame++)
                    {
                        var time = TimeSpan.FromSeconds(1 / framesPerSecond * frame);

                        string fileName = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(options.OutFile),
                            $"{System.IO.Path.GetFileNameWithoutExtension(options.OutFile)}{frame}{System.IO.Path.GetExtension(options.OutFile)}");

                        if (!IsSuccessful(() =>
                            {
                                previousFrame = RenderAnimationAt(animation, time, previousFrame, fileName);
                            }, () =>
                            {
                                Write("Rendered frame ", $"{frame + 1}", ConsoleColor.Gray, ConsoleColor.White);
                                Write(" from ", $"{totalFrames}", ConsoleColor.Gray, ConsoleColor.White);
                            }))
                        {
                            Console.WriteLine("Failed to render animation!");
                            state.Break();
                            return;
                        }
                    }

                    lock (sharedDataLock)
                    {
                        availableMovies.Push(animation);
                    }
                });
        }

        static void Main(string[] args)
        {
            using (new DisposableStopwatch(() => Write("Finished!", ConsoleColor.Green)))
            {
                Parser.Default.ParseArguments<RenderOptions, RenderFrameOptions, RenderParallelOptions>(args)
                    .WithParsed<RenderOptions>(Render)
                    .WithParsed<RenderParallelOptions>(RenderParallel)
                    .WithParsed<RenderFrameOptions>(RenderFrame);
            }

            Console.ForegroundColor = ConsoleColor.Gray;
        }
    }
}
