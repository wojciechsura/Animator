using Animator.Engine.Base;
using Animator.Engine.Base.Persistence;
using Animator.Engine.Elements.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Engine.Elements
{
    /// <summary>
    /// Contains description of a single scene in animation.
    /// </summary>
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
            using var bufferRepository = new BitmapBufferRepository(bitmap.Width, bitmap.Height, bitmap.PixelFormat);
            Render(bitmap, bufferRepository);
        }

        public void Render(Bitmap bitmap, BitmapBufferRepository buffers)
        {
            if (buffers.Width != bitmap.Width || buffers.Height != bitmap.Height || buffers.PixelFormat != bitmap.PixelFormat)
                throw new ArgumentException(nameof(buffers), "Buffer repository bitmap parameters doesn't match output bitmap ones!");

            // Prepare bitmap
            using Graphics graphics = Graphics.FromImage(bitmap);
            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            graphics.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
            graphics.Clear(Color.Transparent);

            if (IsPropertySet(BackgroundProperty))
            {
                using var brush = Background.BuildBrush();
                graphics.FillRectangle(brush, new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height));
            }

            BitmapBuffer buffer = buffers.Lease(graphics.Transform);
            
            try
            {
                foreach (var item in Items)
                {
                    buffer.Graphics.Clear(Color.Transparent);
                    item.Render(buffer, buffers);
                    graphics.DrawImage(buffer.Bitmap, 0, 0);
                }
            }
            finally
            {
                buffers.Return(buffer);
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

        public (ManagedObject, ManagedProperty) FindProperty(string targetName, string path)
        {
            // Find uniquely named component
            ManagedObject element = FindSingleByName(targetName);

            if (element == null)
                return (null, null);

            // Travel through properties in path (so that A.B.C property chaining is possible)
            var props = path.Split('.');
            if (props.Length == 0)
                return (null, null);

            // Get access to first property
            var property = element.GetProperty(props[0]);
            if (property == null)
                return (null, null);

            // Process next properties
            for (int i = 1; i < props.Length; i++)
            {
                // Value of the property must be a ManagedObject
                var value = element.GetValue(property);
                if (value is not ManagedObject)
                    return (null, null);

                element = value as ManagedObject;
                property = element.GetProperty(props[i]);
                if (property == null)
                    return (null, null);
            }

            return (element, property);
        }

        // Public properties --------------------------------------------------

        #region Duration managed property

        /// <summary>
        /// Defines duration of the scene.
        /// </summary>
        public TimeSpan Duration
        {
            get => (TimeSpan)GetValue(DurationProperty);
            set => SetValue(DurationProperty, value);
        }

        public static readonly ManagedProperty DurationProperty = ManagedProperty.Register(typeof(Scene),
            nameof(Duration),
            typeof(TimeSpan),
            new ManagedSimplePropertyMetadata { DefaultValue = TimeSpan.FromSeconds(10) });

        #endregion

        #region Items managed collection

        /// <summary>
        /// Contains all visual elements placed on the scene.
        /// </summary>
        public ManagedCollection<Visual> Items
        {
            get => (ManagedCollection<Visual>)GetValue(ItemsProperty);
        }

        public static readonly ManagedProperty ItemsProperty = ManagedProperty.RegisterCollection(typeof(Scene),
            nameof(Items),
            typeof(ManagedCollection<Visual>));

        #endregion

        #region Animators managed collection

        /// <summary>
        /// Contains list of all animators, which animate properties
        /// of elements placed on the scene.
        /// </summary>
        public ManagedCollection<BaseAnimator> Animators
        {
            get => (ManagedCollection<BaseAnimator>)GetValue(AnimatorsProperty);
        }

        public static readonly ManagedProperty AnimatorsProperty = ManagedProperty.RegisterCollection(typeof(Scene),
            nameof(Animators),
            typeof(ManagedCollection<BaseAnimator>));

        #endregion

        #region Background managed property

        /// <summary>
        /// Defines fill of the background of the scene.
        /// </summary>
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
