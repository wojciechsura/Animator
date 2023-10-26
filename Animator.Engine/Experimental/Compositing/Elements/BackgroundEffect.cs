using Animator.Engine.Base;
using Animator.Engine.Elements.Types;
using Animator.Engine.Elements.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Engine.Elements
{
    /// <summary>
    /// An effect, which is added to the background of
    /// an item after its rendering and before compositing
    /// with the background.
    /// Requires <b>composite rendering</b>.
    /// </summary>
    public abstract class BackgroundEffect : SceneElement
    {
        protected abstract void InternalApply(BitmapBuffer background, BitmapBuffer mask, bool useAlpha, BitmapBufferRepository buffers);

        internal void Apply(BitmapBuffer background, BitmapBuffer foreground, BitmapBuffer mask, BitmapBufferRepository buffers)
        {
            BitmapBuffer usedMask = MaskSource switch
            {
                BackgroundEffectMaskSource.ImageAlpha or
                BackgroundEffectMaskSource.ImageNonTransparent => foreground,
                BackgroundEffectMaskSource.MaskAlpha or
                BackgroundEffectMaskSource.MaskNonTransparent => mask,
                _ => throw new InvalidEnumArgumentException("Unsupported background effect mask source!")
            };

            bool useAlpha = MaskSource switch
            {
                BackgroundEffectMaskSource.ImageAlpha or
                BackgroundEffectMaskSource.MaskAlpha => true,
                BackgroundEffectMaskSource.ImageNonTransparent or
                BackgroundEffectMaskSource.MaskNonTransparent => false,
                _ => throw new InvalidEnumArgumentException("Unsupported background effect mask source!")
            };

            InternalApply(background, usedMask, useAlpha, buffers);
        }

        #region BackgroundEffectMaskSource managed property

        public BackgroundEffectMaskSource MaskSource
        {
            get => (BackgroundEffectMaskSource)GetValue(MaskSourceProperty);
            set => SetValue(MaskSourceProperty, value);
        }

        public static readonly ManagedProperty MaskSourceProperty = ManagedProperty.Register(typeof(BackgroundEffect),
            nameof(MaskSource),
            typeof(BackgroundEffectMaskSource),
            new ManagedSimplePropertyMetadata { DefaultValue = BackgroundEffectMaskSource.ImageNonTransparent });

        #endregion
    }
}
