﻿using Animator.Engine.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Engine.Elements
{
    public class Pen : BaseElement
    {
        // Internal methods ---------------------------------------------------

        internal System.Drawing.Pen BuildPen()
        {
            return new System.Drawing.Pen(Color, Width);
        }

        // Public properties --------------------------------------------------

        #region Color managed property

        public System.Drawing.Color Color
        {
            get => (System.Drawing.Color)GetValue(ColorProperty);
            set => SetValue(ColorProperty, value);
        }

        public static readonly ManagedProperty ColorProperty = ManagedProperty.Register(typeof(Pen),
            nameof(Color),
            typeof(System.Drawing.Color),
            new ManagedSimplePropertyMetadata(System.Drawing.Color.Black));

        #endregion

        #region Width managed property

        public float Width
        {
            get => (float)GetValue(WidthProperty);
            set => SetValue(WidthProperty, value);
        }

        public static readonly ManagedProperty WidthProperty = ManagedProperty.Register(typeof(Pen),
            nameof(Width),
            typeof(float),
            new ManagedSimplePropertyMetadata(1.0f));

        #endregion
    }
}
