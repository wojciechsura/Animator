using Animator.Engine.Base;
using Animator.Engine.Elements.Types;
using Animator.Engine.Elements.Utilities;
using Animator.Engine.Exceptions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Engine.Elements
{
    /// <summary>
    /// Draws a text on a scene.
    /// </summary>
    public class Label : Visual
    {
        // Protected properties -----------------------------------------------

        protected override void InternalRender(BitmapBuffer buffer, BitmapBufferRepository buffers)
        {
            if (!IsPropertySet(BrushProperty))
                return;

            using System.Drawing.Brush brush = Brush.BuildBrush();

            FontFamily fontFamily = System.Drawing.FontFamily.Families.FirstOrDefault(ff => ff.Name == FontFamily)
                ?? throw new AnimationException($"Cannot find font family {FontFamily}.", GetPath());

            FontStyle fontStyle = 0;
            if (Bold)
                fontStyle |= FontStyle.Bold;
            if (Italic)
                fontStyle |= FontStyle.Italic;
            if (Underline)
                fontStyle |= FontStyle.Underline;
            
            using var font = new Font(fontFamily, FontSize, fontStyle, GraphicsUnit.Pixel);

            SizeF size = buffer.Graphics.MeasureString(Text, font);

            float x = HorizontalAlignment switch
            {
                HorizontalAlignment.Left => 0.0f,
                HorizontalAlignment.Center => -size.Width / 2.0f,
                HorizontalAlignment.Right => -size.Width,
                _ => throw new InvalidOperationException("Unsupported horizontal alignment!")
            };

            float y = VerticalAlignment switch
            {
                VerticalAlignment.Top => 0.0f,
                VerticalAlignment.Center => -size.Height / 2.0f,
                VerticalAlignment.Bottom => -size.Height,
                _ => throw new InvalidOperationException("Unsupported vertical alignment")
            };

            var path = new GraphicsPath();
            path.AddString(Text, fontFamily, (int)fontStyle, FontSize, new PointF(x, y), null);
            path.FillMode = FillMode.Winding;
            buffer.Graphics.FillPath(brush, path);

            // buffer.Graphics.DrawString(Text, font, brush, new PointF(x, y));
        }

        // Public properties --------------------------------------------------

        #region Brush managed property

        /// <summary>
        /// Defines fill of drawn text.
        /// </summary>
        public Brush Brush
        {
            get => (Brush)GetValue(BrushProperty);
            set => SetValue(BrushProperty, value);
        }

        public static readonly ManagedProperty BrushProperty = ManagedProperty.RegisterReference(typeof(Label),
            nameof(Brush),
            typeof(Brush),
            new ManagedReferencePropertyMetadata());

        #endregion

        #region Text managed property

        /// <summary>
        /// Text to be drawn.
        /// </summary>
        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public static readonly ManagedProperty TextProperty = ManagedProperty.Register(typeof(Label),
            nameof(Text),
            typeof(string),
            new ManagedSimplePropertyMetadata { DefaultValue = "" });

        #endregion

        #region Font managed property

        /// <summary>
        /// Defines font family to be used while drawing text.
        /// </summary>
        public string FontFamily
        {
            get => (string)GetValue(FontFamilyProperty);
            set => SetValue(FontFamilyProperty, value);
        }

        public static readonly ManagedProperty FontFamilyProperty = ManagedProperty.Register(typeof(Label),
            nameof(FontFamily),
            typeof(string),
            new ManagedSimplePropertyMetadata { DefaultValue = "Calibri" });

        #endregion

        #region FontSize managed property

        /// <summary>
        /// Defines font size of the drawn text.
        /// </summary>
        /// <remarks>
        /// Use SizeUnit property to define unit (pixels or points)
        /// </remarks>
        public float FontSize
        {
            get => (float)GetValue(FontSizeProperty);
            set => SetValue(FontSizeProperty, value);
        }

        public static readonly ManagedProperty FontSizeProperty = ManagedProperty.Register(typeof(Label),
            nameof(FontSize),
            typeof(float),
            new ManagedSimplePropertyMetadata { DefaultValue = 11.0f });

        #endregion

        #region Bold managed property

        /// <summary>
        /// Set this property to true to make text bold.
        /// </summary>
        public bool Bold
        {
            get => (bool)GetValue(BoldProperty);
            set => SetValue(BoldProperty, value);
        }

        public static readonly ManagedProperty BoldProperty = ManagedProperty.Register(typeof(Label),
            nameof(Bold),
            typeof(bool),
            new ManagedSimplePropertyMetadata { DefaultValue = false });

        #endregion

        #region Italic managed property

        /// <summary>
        /// Set this property to true to make text italic.
        /// </summary>
        public bool Italic
        {
            get => (bool)GetValue(ItalicProperty);
            set => SetValue(ItalicProperty, value);
        }

        public static readonly ManagedProperty ItalicProperty = ManagedProperty.Register(typeof(Label),
            nameof(Italic),
            typeof(bool),
            new ManagedSimplePropertyMetadata { DefaultValue = false });

        #endregion

        #region Underline managed property

        /// <summary>
        /// Set this property to true to make text underlined.
        /// </summary>
        public bool Underline
        {
            get => (bool)GetValue(UnderlineProperty);
            set => SetValue(UnderlineProperty, value);
        }

        public static readonly ManagedProperty UnderlineProperty = ManagedProperty.Register(typeof(Label),
            nameof(Underline),
            typeof(bool),
            new ManagedSimplePropertyMetadata { DefaultValue = false });

        #endregion

        #region HorizontalAlignment

        public HorizontalAlignment HorizontalAlignment
        {
            get => (HorizontalAlignment)GetValue(HorizontalAlignmentProperty);
            set => SetValue(HorizontalAlignmentProperty, value);
        }

        public static readonly ManagedProperty HorizontalAlignmentProperty = ManagedProperty.Register(typeof(Label),
            nameof(HorizontalAlignment),
            typeof(HorizontalAlignment),
            new ManagedSimplePropertyMetadata { DefaultValue = HorizontalAlignment.Left });

        #endregion

        #region VerticalAlignment

        public VerticalAlignment VerticalAlignment
        {
            get => (VerticalAlignment)GetValue(VerticalAlignmentProperty);
            set => SetValue(VerticalAlignmentProperty, value);
        }

        public static readonly ManagedProperty VerticalAlignmentProperty = ManagedProperty.Register(typeof(Label),
            nameof(VerticalAlignment),
            typeof(VerticalAlignment),
            new ManagedSimplePropertyMetadata { DefaultValue = VerticalAlignment.Top });

        #endregion
    }
}
