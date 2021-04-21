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

        public static readonly ManagedProperty BackgroundProperty = ManagedProperty.Register(typeof(Scene),
            nameof(Background),
            typeof(Brush),
            new ManagedSimplePropertyMetadata(null));

        #endregion
    }
}
