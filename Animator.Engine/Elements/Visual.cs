using Animator.Engine.Base;
using Animator.Engine.Elements.Utilities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Engine.Elements
{
    public abstract class Visual : BaseElement
    {
        protected abstract void InternalRender(BitmapBuffer buffer);

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

        internal void Render(BitmapBuffer buffer, BitmapBufferRepository buffers)
        {
            var originalTransform = buffer.Graphics.Transform;

            var transform = originalTransform.Clone();
            transform.Multiply(BuildTransformMatrix(), MatrixOrder.Prepend);
            buffer.Graphics.Transform = transform;

            InternalRender(buffer);

            buffer.Graphics.Transform = originalTransform;
        }

        #region Position managed property

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
    }
}
