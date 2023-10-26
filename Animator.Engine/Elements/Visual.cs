using Animator.Engine.Base;
using Animator.Engine.Elements.Rendering;
using Animator.Engine.Elements.Types;
using Animator.Engine.Elements.Utilities;
using Animator.Engine.Tools;
using Animator.Engine.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Engine.Elements
{
    /// <summary>
    /// Represents basic element, which can be drawn on the scene.
    /// </summary>
    public abstract partial class Visual : SceneElement
    {
        // Protected methods --------------------------------------------------

        protected abstract void InternalRender(BitmapBuffer buffer, BitmapBufferRepository buffers, RenderingContext context);

        protected Matrix BuildTransformMatrix()
        {
            var result = new Matrix();

            if (IsPropertySet(OriginProperty))
                result.Translate(-Origin.X, -Origin.Y, MatrixOrder.Append);

            if (IsPropertySet(RotationProperty))
                result.Rotate(Rotation, MatrixOrder.Append);

            if (IsPropertySet(ScaleProperty))
                result.Scale(Scale.X, Scale.Y, MatrixOrder.Append);

            result.Translate(Position.X, Position.Y, MatrixOrder.Append);

            return result;
        }

        // Internal methods ---------------------------------------------------

        internal void Render(BitmapBuffer buffer, BitmapBufferRepository buffers, RenderingContext context)
        {
            if (!Visible || Alpha.IsZero())
                return;

            var originalTransform = buffer.Graphics.Transform;

            var transform = originalTransform.Clone();
            transform.Multiply(BuildTransformMatrix(), MatrixOrder.Prepend);

            // Scale eqaual to 0 equals to invisible object
            if (Math.Abs(transform.MatrixElements.M11).IsZero() &&
                Math.Abs(transform.MatrixElements.M12).IsZero() &&
                Math.Abs(transform.MatrixElements.M21).IsZero() &&
                Math.Abs(transform.MatrixElements.M22).IsZero())
                return;

            buffer.Graphics.Transform = transform;

            InternalRender(buffer, buffers, context);

            if (IsPropertySet(AlphaProperty))
                VisualRenderer.ApplyAlpha(Alpha, buffer);

            VisualRenderer.ApplyEffects(buffer, buffers, Effects);

            VisualRenderer.ApplyMask(buffer, originalTransform, transform, Mask, MaskCoordinateSystem, InvertMask, context, buffers);

            buffer.Graphics.Transform = originalTransform;

            context?.AfterRendering(this, buffer.Bitmap);
        }
       
        // Public properties --------------------------------------------------

        #region Visible managed property

        /// <summary>
        /// Defines, whether element is visible or not.
        /// </summary>
        public bool Visible
        {
            get => (bool)GetValue(VisibleProperty);
            set => SetValue(VisibleProperty, value);
        }

        public static readonly ManagedProperty VisibleProperty = ManagedProperty.Register(typeof(Visual),
            nameof(Visible),
            typeof(bool),
            new ManagedSimplePropertyMetadata { DefaultValue = true });

        #endregion

        #region Position managed property

        /// <summary>
        /// Position of this visual on the scene.
        /// </summary>
        public PointF Position
        {
            get => (PointF)GetValue(PositionProperty);
            set => SetValue(PositionProperty, value);
        }

        public static readonly ManagedProperty PositionProperty = ManagedProperty.Register(typeof(Visual),
            nameof(Position),
            typeof(PointF),
            new ManagedSimplePropertyMetadata { DefaultValue = new PointF(0.0f, 0.0f) });

        #endregion

        #region Origin managed property

        /// <summary>
        /// An anchor point of this visual. It affects numerous transforms, 
        /// such as Position, Rotation and Scale. In general, it defines a
        /// point of the visual, which will be placed on the scene in place
        /// defined by Position property. Also, it defines center point
        /// for rotation and scaling.
        /// </summary>
        public PointF Origin
        {
            get => (PointF)GetValue(OriginProperty);
            set => SetValue(OriginProperty, value);
        }

        public static readonly ManagedProperty OriginProperty = ManagedProperty.Register(typeof(Visual),
            nameof(Origin),
            typeof(PointF),
            new ManagedSimplePropertyMetadata { DefaultValue = new PointF(0.0f, 0.0f) });

        #endregion

        #region Rotation managed property

        /// <summary>
        /// Rotates visual by specific angle.
        /// </summary>
        public float Rotation
        {
            get => (float)GetValue(RotationProperty);
            set => SetValue(RotationProperty, value);
        }

        public static readonly ManagedProperty RotationProperty = ManagedProperty.Register(typeof(Visual),
            nameof(Rotation),
            typeof(float),
            new ManagedSimplePropertyMetadata { DefaultValue = 0.0f });

        #endregion

        #region Scale managed property

        /// <summary>
        /// Scales the visual by specified factor (separately
        /// in X and Y axis).
        /// </summary>
        public PointF Scale
        {
            get => (PointF)GetValue(ScaleProperty);
            set => SetValue(ScaleProperty, value);
        }

        public static readonly ManagedProperty ScaleProperty = ManagedProperty.Register(typeof(Visual),
            nameof(Scale),
            typeof(PointF),
            new ManagedSimplePropertyMetadata { DefaultValue = new PointF(1.0f, 1.0f) });

        #endregion

        #region Alpha managed property

        /// <summary>
        /// Applies transparency to a visual.
        /// </summary>
        public float Alpha
        {
            get => (float)GetValue(AlphaProperty);
            set => SetValue(AlphaProperty, value);
        }

        public static readonly ManagedProperty AlphaProperty = ManagedProperty.Register(typeof(Visual),
            nameof(Alpha),
            typeof(float),
            new ManagedSimplePropertyMetadata 
            { 
                DefaultValue = 1.0f,
                CoerceValueHandler = CoerceAlpha
            });

        private static object CoerceAlpha(ManagedObject obj, object baseValue)
        {
            var value = (float)baseValue;
            return Math.Max(0.0f, Math.Min(1.0f, value));
        }

        #endregion

        #region Effects managed collection

        /// <summary>
        /// List of effects, which will be applied to
        /// a visual after rendering.
        /// </summary>
        public ManagedCollection<Effect> Effects
        {
            get => (ManagedCollection<Effect>)GetValue(EffectsProperty);
        }

        public static readonly ManagedProperty EffectsProperty = ManagedProperty.RegisterCollection(typeof(Visual),
            nameof(Effects),
            typeof(ManagedCollection<Effect>));

        #endregion

        #region InvertMask managed property

        /// <summary>
        /// Defines, if mask expresses areas, which should or should not
        /// be drawn. Useful, if you want to e.g. draw everything
        /// <em>except</em> some specific area.
        /// </summary>
        public bool InvertMask
        {
            get => (bool)GetValue(InvertMaskProperty);
            set => SetValue(InvertMaskProperty, value);
        }

        public static readonly ManagedProperty InvertMaskProperty = ManagedProperty.Register(typeof(Visual),
            nameof(InvertMask),
            typeof(bool),
            new ManagedSimplePropertyMetadata { DefaultValue = false });

        #endregion

        #region MaskCoordinateSystem managed property

        /// <summary>
        /// Defines, in which coordinates the Visual's mask is defined. If you set this property to Local,
        /// mask will follow all transforms applied to the Visual. If you set to Global, it will be immune
        /// to those transforms.
        /// </summary>
        public MaskCoordinateSystem MaskCoordinateSystem
        {
            get => (MaskCoordinateSystem)GetValue(MaskCoordinateSystemProperty);
            set => SetValue(MaskCoordinateSystemProperty, value);
        }

        public static readonly ManagedProperty MaskCoordinateSystemProperty = ManagedProperty.Register(typeof(Visual),
            nameof(MaskCoordinateSystem),
            typeof(MaskCoordinateSystem),
            new ManagedSimplePropertyMetadata { DefaultValue = MaskCoordinateSystem.Local });

        #endregion

        #region Mask managed collection

        /// <summary>
        /// Allows masking a visual, e.g. drawing only parts of it on
        /// a screen. Use other visuals to describe the mask, although
        /// only alpha-value of the final mask is used (colors does
        /// not matter). If mask is not inverted (InvertMask property),
        /// areas of the visual, where its mask has value of 1 are drawn
        /// completely and areas, where the mask has value of 0 are not
        /// drawn at all (with all values in between allowing partial 
        /// drawing). If mask is inverted, this behavior works the other
        /// way around.
        /// </summary>
        public ManagedCollection<Visual> Mask
        {
            get => (ManagedCollection<Visual>)GetValue(MaskProperty);
        }

        public static readonly ManagedProperty MaskProperty = ManagedProperty.RegisterCollection(typeof(Visual),
            nameof(Mask),
            typeof(ManagedCollection<Visual>));

        #endregion
    }
}
