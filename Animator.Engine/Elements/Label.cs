using Animator.Engine.Base;
using Animator.Engine.Elements.Types;
using Animator.Engine.Elements.Utilities;
using Animator.Engine.Exceptions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
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

            System.Drawing.FontFamily fontFamily = System.Drawing.FontFamily.Families.FirstOrDefault(ff => ff.Name == FontFamily);
            if (fontFamily == null)
                throw new AnimationException($"Cannot find font family {FontFamily}.", GetPath());

            FontStyle fontStyle = 0;
            if (Bold)
                fontStyle |= FontStyle.Bold;
            if (Italic)
                fontStyle |= FontStyle.Italic;
            if (Underline)
                fontStyle |= FontStyle.Underline;
            
            var unit = SizeUnit switch
            {
                FontSizeUnit.Pixels => GraphicsUnit.Pixel,
                FontSizeUnit.Points => GraphicsUnit.Point,
                _ => throw new InvalidEnumArgumentException("Unsupported font size unit!"),
            };

            using var font = new Font(fontFamily, FontSize, fontStyle, unit);
            buffer.Graphics.DrawString(Text, font, brush, Position);
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

        #region SizeUnit managed property

        /// <summary>
        /// Defines unit in which size of font is expressed.
        /// Possible values:
        /// <ul>
        ///   <li>Points</li>
        ///   <li>Pixels</li>
        /// </ul>
        /// </summary>
        public FontSizeUnit SizeUnit
        {
            get => (FontSizeUnit)GetValue(SizeUnitProperty);
            set => SetValue(SizeUnitProperty, value);
        }

        public static readonly ManagedProperty SizeUnitProperty = ManagedProperty.Register(typeof(Label),
            nameof(SizeUnit),
            typeof(FontSizeUnit),
            new ManagedSimplePropertyMetadata { DefaultValue = FontSizeUnit.Points });

        #endregion
    }
}
