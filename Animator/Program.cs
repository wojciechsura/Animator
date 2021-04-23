using Animator.Engine.Animation;
using Animator.Engine.Animation.Maths;
using Animator.Engine.Elements;
using Animator.Engine.Elements.Persistence;
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
        static void Main(string[] args)
        {
            // Placeholder for tests          
            XmlDocument document = new();
            document.Load(args[0]);

            var animationSerializer = new AnimationSerializer();
            var animation = animationSerializer.Deserialize(document);

            // Render scene without animation

            var bitmap = new Bitmap(animation.Config.Width, animation.Config.Height, PixelFormat.Format32bppArgb);
            animation.Scenes[0].Render(bitmap);
            bitmap.Save(@"D:\scene.png");

            // Render animation

            var frameCount = TimeCalculator.EvalFrameCount((float)animation.Scenes[0].Duration.TotalMilliseconds, animation.Config.FramesPerSecond);

            for (int i = 0; i < frameCount; i++)
            {
                float timeMs = TimeCalculator.EvalMillisecondsForFrame(i, animation.Config.FramesPerSecond);

                foreach (var animator in animation.Scenes[0].Animators)
                    animator.ApplyAnimation(timeMs);

                animation.Scenes[0].Render(bitmap);

                bitmap.Save($"D:\\frame{i}.png");
            }
        }
    }
}
