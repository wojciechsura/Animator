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

            string testAnimation =
"<Animation>" +
"  <Animation.Config>" +
"    <AnimationConfig Width=\"100\" Height=\"100\" />" +
"  </Animation.Config>" +
"  <Scene Name=\"Scene1\">" +
"    <Rectangle Position=\"5;5\" Rotation=\"-10\" Width=\"20\" Height=\"20\" Brush=\"Green\">" +
"      <Rectangle.Pen>" +
"        <Pen Color=\"Black\" Width=\"1\" />" +
"      </Rectangle.Pen>" +
"    </Rectangle>" +
"    <Rectangle Position=\"10;10\" Width=\"20\" Height=\"20\" Brush=\"Red\">" +
"      <Rectangle.Pen>" +
"        <Pen Color=\"Black\" Width=\"1\" />" +
"      </Rectangle.Pen>" +
"    </Rectangle>" +
"  </Scene>" +
"</Animation>";

            XmlDocument document = new();
            document.LoadXml(testAnimation);

            var animationSerializer = new AnimationSerializer();
            var animation = animationSerializer.Deserialize(document);

            Dictionary<string, List<BaseElement>> names = new();
            animation.Scenes.First().FindNamedElements(names);

            foreach (var kvp in names)
            {
                Console.WriteLine($"{kvp.Key} - {kvp.Value.Count()} item(s)");
            }

            var bitmap = new Bitmap(animation.Config.Width, animation.Config.Height, PixelFormat.Format32bppArgb);
            animation.Scenes[0].Render(bitmap);

            bitmap.Save(@"D:\scene.png");
        }
    }
}
