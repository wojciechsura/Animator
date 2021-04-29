using Animator.Engine.Animation.Maths;
using Animator.Engine.Base;
using Animator.Engine.Elements.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Engine.Elements
{
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

        public string Key => key;
    }
}
