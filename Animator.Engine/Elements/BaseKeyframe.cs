using Animator.Engine.Animation.Maths;
using Animator.Engine.Base;
using Animator.Engine.Elements.Types;
using Animator.Engine.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Engine.Elements
{
    /// <summary>
    /// Base class for all keyframe classes. A keyframe
    /// class describes fixed value for a property in 
    /// specific time.
    /// </summary>
    public abstract class BaseKeyframe : BaseElement
    {
        private string key;

        // Private static methods ---------------------------------------------

        private void UpdateKey()
        {
            key = $"{TargetName}.{Path}";
        }

        // Public methods -----------------------------------------------------

        public abstract object GetValue();

        public abstract object EvalValue(float fromTimeMs, object fromValue, float currentTimeMs);

        // Public properties --------------------------------------------------

        #region Time managed property

        /// <summary>
        /// Define exact time since start of a scene, when a
        /// property should be set to a fixed value.
        /// </summary>
        public TimeSpan Time
        {
            get => (TimeSpan)GetValue(TimeProperty);
            set => SetValue(TimeProperty, value);
        }

        public static readonly ManagedProperty TimeProperty = ManagedProperty.Register(typeof(BaseKeyframe),
            nameof(Time),
            typeof(TimeSpan),
            new ManagedSimplePropertyMetadata { DefaultValue = TimeSpan.FromMilliseconds(0) });

        #endregion

        #region TargetName managed property

        /// <summary>
        /// Defines name of an object, which property should be modified.
        /// </summary>
        public string TargetName
        {
            get => (string)GetValue(TargetNameProperty);
            set => SetValue(TargetNameProperty, value);
        }

        public static readonly ManagedProperty TargetNameProperty = ManagedProperty.Register(typeof(BaseKeyframe),
            nameof(TargetName),
            typeof(string),
            new ManagedSimplePropertyMetadata { NotAnimatable = true, Inheritable = true, InheritedFromParent = true, ValueChangedHandler = HandleTargetNameChanged });

        private static void HandleTargetNameChanged(ManagedObject sender, PropertyValueChangedEventArgs args)
        {
            if (sender is BaseKeyframe baseKeyframe)
                baseKeyframe.UpdateKey();
        }

        #endregion

        #region Path managed property

        /// <summary>
        /// Defines path to a property, starting at the object pointed
        /// to by TargetName. Path may be either a single property,
        /// for example <code>Position</code>, or a chain of properties,
        /// leading through subsequent object, like <code>Pen.Color</code>.
        /// </summary>
        public string Path
        {
            get => (string)GetValue(PathProperty);
            set => SetValue(PathProperty, value);
        }

        public static readonly ManagedProperty PathProperty = ManagedProperty.Register(typeof(BaseKeyframe),
            nameof(Path),
            typeof(string),
            new ManagedSimplePropertyMetadata { NotAnimatable = true, Inheritable = true, InheritedFromParent = true, ValueChangedHandler = HandlePathChanged });

        private static void HandlePathChanged(ManagedObject sender, PropertyValueChangedEventArgs args)
        {
            if (sender is BaseKeyframe baseKeyframe)
                baseKeyframe.UpdateKey();
        }

        #endregion

        [DoNotDocument]
        public string Key => key;
    }
}
