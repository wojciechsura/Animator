using Animator.Engine.Animation;
using Animator.Engine.Animation.Maths;
using Animator.Engine.Elements;
using Animator.Engine.Elements.Persistence;
using Animator.Options;
using CommandLine;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Xml;

namespace Animator
{
    class Program
    {
        private static Animation LoadAnimation(string path)
        {
            XmlDocument document = new();
            document.Load(path);

            AnimationSerializer animationSerializer = new AnimationSerializer();
            Animation animation = animationSerializer.Deserialize(document);

            return animation;
        }

        private static void DisplayError(Exception e)
        {
            Console.WriteLine("Reason:");

            while (e != null)
            {
                Console.WriteLine(e.Message);
                if (e.InnerException != null)
                    Console.WriteLine("Because:");

                e = e.InnerException;
            }
        }

        private static bool IsSuccessful(Action action, string message)
        {
            using (new DisposableStopwatch(message))
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
                Console.WriteLine("Failed to perform requested operation!");
                DisplayError(e);
                return false;
            }
        }

        private static void RenderAnimationAt(Animation animation, TimeSpan time, string outFile)
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

            foreach (var animator in animation.Scenes[i].Animators)
                animator.ApplyAnimation((float)sceneTimeOffset.TotalMilliseconds);

            var bitmap = new Bitmap(animation.Config.Width, animation.Config.Height, PixelFormat.Format32bppArgb);

            animation.Scenes[i].Render(bitmap);

            bitmap.Save(outFile);
        }

        private static void RenderFrame(RenderFrameOptions options)
        {
            Animation animation = null;


            if (!IsSuccessful(() => { animation = LoadAnimation(options.Source); }, "Loaded animation"))
                return;

            var framesPerSecond = animation.Config.FramesPerSecond;

            if (options.FrameIndex != null)
            {
                TimeSpan time = TimeSpan.FromSeconds(1 / framesPerSecond * options.FrameIndex.Value);

                if (!IsSuccessful(() => RenderAnimationAt(animation, time, options.OutFile), "Rendered single frame"))
                    return;
                
            }
            else if (options.TimeOffset != null)
            {
                if (!IsSuccessful(() => RenderAnimationAt(animation, options.TimeOffset.Value, options.OutFile), "Rendered single frame"))
                    return;
            }
        }

        private static void Render(RenderOptions options)
        {
            Animation animation = null;

            if (!IsSuccessful(() => { animation = LoadAnimation(options.Source); }))
                return;

            var framesPerSecond = animation.Config.FramesPerSecond;
            var totalAnimationTime = animation.Scenes.Sum(s => s.Duration.TotalMilliseconds);
            var totalFrames = (int)(totalAnimationTime / 1000.0f * framesPerSecond);

            for (int frame = 0; frame < totalFrames; frame++)
            {
                var time = TimeSpan.FromSeconds(1 / framesPerSecond * frame);

                string fileName = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(options.OutFile),
                    $"{System.IO.Path.GetFileNameWithoutExtension(options.OutFile)}{frame}{System.IO.Path.GetExtension(options.OutFile)}");

                if (!IsSuccessful(() => RenderAnimationAt(animation, time, fileName), $"Rendered frame {frame + 1} from {totalFrames}"))
                    return;
            }
        }

        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<RenderOptions, RenderFrameOptions>(args)
                .WithParsed<RenderOptions>(Render)
                .WithParsed<RenderFrameOptions>(RenderFrame);            
        }
    }
}
