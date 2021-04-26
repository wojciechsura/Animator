using Animator.Engine.Animation;
using Animator.Engine.Animation.Maths;
using Animator.Engine.Elements;
using Animator.Engine.Elements.Persistence;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Xml;

namespace Animator
{
    public class DisposableStopwatch : IDisposable
    {
        private readonly string comment;
        private readonly Stopwatch stopwatch;

        public DisposableStopwatch(string comment)
        {
            this.comment = comment;
            stopwatch = Stopwatch.StartNew();
        }

        public void Dispose()
        {
            stopwatch.Stop();
            Console.WriteLine($"{comment} took {stopwatch.ElapsedMilliseconds}ms ({stopwatch.Elapsed})");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            // Placeholder for tests          
            XmlDocument document = new();
            using (new DisposableStopwatch("Loading XML file"))
            {
                document.Load(args[0]);
            }

            AnimationSerializer animationSerializer;
            Animation animation;

            using (new DisposableStopwatch("Loading animation"))
            {
                animationSerializer = new AnimationSerializer();
                animation = animationSerializer.Deserialize(document);
            }

            Random random = new Random();

            // Adding 100 rectangles
            // Add100Rectangles(animation, random);

            // Render scene without animation

            var bitmap = new Bitmap(animation.Config.Width, animation.Config.Height, PixelFormat.Format32bppArgb);
            // animation.Scenes[0].Render(bitmap);
            // bitmap.Save(@"D:\scene.png");

            // Render animation

            var frameCount = TimeCalculator.EvalFrameCount((float)animation.Scenes[0].Duration.TotalMilliseconds, animation.Config.FramesPerSecond);

            for (int i = 0; i < frameCount; i++)
            {
                using (new DisposableStopwatch("Rendering frame"))
                {
                    Console.WriteLine($"Rendering frame {i} of {frameCount}...");

                    float timeMs = TimeCalculator.EvalMillisecondsForFrame(i, animation.Config.FramesPerSecond);

                    Console.WriteLine($"  Applying animators...");
                    foreach (var animator in animation.Scenes[0].Animators)
                        animator.ApplyAnimation(timeMs);

                    Console.WriteLine($"  Rendering scene...");
                    animation.Scenes[0].Render(bitmap);

                    Console.WriteLine($"  Saving file...");
                    bitmap.Save($"D:\\frame{i}.png");
                }
            }
        }

        private static void Add100Rectangles(Animation animation, Random random)
        {
            for (int i = 0; i < 100; i++)
            {
                float width = random.Next(100);
                float height = random.Next(100);

                var rect = new Animator.Engine.Elements.Rectangle
                {
                    Name = $"Rectangle{i}",
                    Alpha = random.Next(100) / 100.0f,
                    Width = 20,
                    Height = 20,
                    Origin = new PointF(10, 10),
                    Position = new PointF(random.Next(100), random.Next(100)),
                    Rotation = random.Next(360),
                    Brush = new Animator.Engine.Elements.SolidBrush
                    {
                        Color = Color.FromArgb(random.Next(255), random.Next(255), random.Next(255))
                    }
                };

                animation.Scenes[0].Items.Insert(0, rect);

                PropertyAnimator animator = new Animator.Engine.Elements.FloatPropertyAnimator
                {
                    TargetName = $"Rectangle{i}",
                    Path = "Rotation",
                    StartTime = TimeSpan.FromSeconds(0),
                    EndTime = TimeSpan.FromSeconds(3),
                    EasingFunction = Engine.Elements.Types.EasingFunction.EaseBackSlowDown,
                    From = random.Next(720) - 360,
                    To = 0
                };

                animation.Scenes[0].Animators.Add(animator);

                animator = new Animator.Engine.Elements.PointPropertyAnimator
                {
                    TargetName = $"Rectangle{i}",
                    Path = "Position",
                    StartTime = TimeSpan.FromSeconds(0),
                    EndTime = TimeSpan.FromSeconds(3),
                    EasingFunction = Engine.Elements.Types.EasingFunction.EaseBackSlowDown,
                    From = new PointF(random.Next(800), random.Next(600)),
                    To = new PointF((i % 10) * 30 + 30, (i / 10) * 30 + 30)
                };

                animation.Scenes[0].Animators.Add(animator);
            }
        }
    }
}
