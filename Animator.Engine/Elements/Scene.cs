using Animator.Engine.Base;
using Animator.Engine.Base.Persistence;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Engine.Elements
{
    [ContentProperty(nameof(Items))]
    public class Scene : BaseElement
    {
        // Private fields -----------------------------------------------------

        private readonly Dictionary<string, List<BaseElement>> names = new();

        // Internal methods ---------------------------------------------------

        internal void RegisterName(string name, BaseElement baseElement)
        {
            if (!names.TryGetValue(name, out List<BaseElement> list))
            {
                list = new List<BaseElement>();
                names[name] = list;
            }

            list.Add(baseElement);
        }

        internal void UnregisterName(string name, BaseElement baseElement)
        {
            if (names.TryGetValue(name, out List<BaseElement> list))
            {
                list.Remove(baseElement);                
                if (!list.Any())
                    names.Remove(name);
            }
        }

        // Public methods -----------------------------------------------------

        public void Render(Bitmap bitmap)
        {
            // Clear bitmap
            using Graphics graphics = Graphics.FromImage(bitmap);
            graphics.Clear(Color.Transparent);

            if (IsPropertySet(BackgroundProperty))
            {
                using var brush = Background.BuildBrush();
                graphics.FillRectangle(brush, new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height));
            }

            foreach (var item in Items)
            {
                item.Render(bitmap, graphics);
            }
        }

        public BaseElement FindSingleByName(string name)
        {
            if (names.TryGetValue(name, out List<BaseElement> elements))
                return elements.SingleOrDefault();

            return null;
        }

        public IEnumerable<BaseElement> FindElements(string name)
        {
            if (names.TryGetValue(name, out List<BaseElement> elements))
                return elements;

            return null;
        }

        // Public properties --------------------------------------------------

        #region Items managed collection

        public List<Visual> Items
        {
            get => (List<Visual>)GetValue(ItemsProperty);
        }

        public static readonly ManagedProperty ItemsProperty = ManagedProperty.RegisterCollection(typeof(Scene),
            nameof(Items),
            typeof(List<Visual>));

        #endregion

        #region Background managed property

        public Brush Background
        {
            get => (Brush)GetValue(BackgroundProperty);
            set => SetValue(BackgroundProperty, value);
        }

        public static readonly ManagedProperty BackgroundProperty = ManagedProperty.RegisterReference(typeof(Scene),
            nameof(Background),
            typeof(Brush));

        #endregion


    }
}
