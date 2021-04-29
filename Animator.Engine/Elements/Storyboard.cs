using Animator.Engine.Base;
using Animator.Engine.Base.Persistence;
using Animator.Engine.Elements.Types;
using Animator.Engine.Exceptions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Engine.Elements
{
    [ContentProperty(nameof(Storyboard.Keyframes))]
    public class Storyboard : BaseAnimator
    {
        // Public methods -----------------------------------------------------

        public override void ApplyAnimation(float timeMs)
        {
            var groups = Keyframes.GroupBy(k => k.Key);
            
            foreach (var group in groups)
            {
                var keyframes = group.OrderBy(k => k.Time).ToList();

                if (keyframes.Any(k => k.GetType() != keyframes[0].GetType()))
                    throw new AnimationException($"All keyframes for element {keyframes[0].Key} and path {keyframes[0].Path} must be of the same type!");

                (var obj, var prop) = Scene.FindProperty(TargetName, Path);
                if (obj == null || prop == null)
                    continue;
                if (prop is not ManagedSimpleProperty simpleProperty)
                    throw new AnimationException($"Property {prop.Name} of object {obj.GetType().Name} is not simple property and thus can not be animated!");

                int i = -1;
                while (i + 1 < keyframes.Count && keyframes[i + 1].Time.TotalMilliseconds <= timeMs)
                    i++;

                if (i == -1)
                {
                    // Before first keyframe
                    object fromValue = simpleProperty.Metadata.DefaultValue;
                    object value = keyframes[0].EvalValue(0, fromValue, timeMs);
                    obj.SetAnimatedValue(prop, value);
                }
                else if (i == keyframes.Count - 1)
                {
                    // Past last keyframe
                    object value = keyframes[i].GetValue();
                    obj.SetAnimatedValue(prop, value);
                }
                else
                {
                    object fromValue = keyframes[i].GetValue();
                    TimeSpan fromTime = keyframes[i].Time;
                    object value = keyframes[i + 1].EvalValue((float)fromTime.TotalMilliseconds, fromValue, timeMs);
                    obj.SetAnimatedValue(prop, value);
                }
            }
        }

        public override void ResetAnimation()
        {
            var groups = Keyframes.GroupBy(k => k.Key);

            foreach (var group in groups)
            {
                (var obj, var prop) = Scene.FindProperty(TargetName, Path);
                if (obj == null || prop == null)
                    continue;
                if (prop is not ManagedSimpleProperty simpleProperty)
                    throw new AnimationException($"Property {prop.Name} of object {obj.GetType().Name} is not simple property and thus can not be animated!");

                obj.ClearAnimatedValue(prop);
            }
        }

        // Public properties --------------------------------------------------

        #region TargetName managed property

        public string TargetName
        {
            get => (string)GetValue(TargetNameProperty);
            set => SetValue(TargetNameProperty, value);
        }

        public static readonly ManagedProperty TargetNameProperty = ManagedProperty.Register(typeof(Storyboard),
            nameof(TargetName),
            typeof(string),
            new ManagedSimplePropertyMetadata { NotAnimatable = true, Inheritable = true, InheritedFromParent = true });

        #endregion

        #region Path managed property

        public string Path
        {
            get => (string)GetValue(PathProperty);
            set => SetValue(PathProperty, value);
        }

        public static readonly ManagedProperty PathProperty = ManagedProperty.Register(typeof(Storyboard),
            nameof(Path),
            typeof(string),
            new ManagedSimplePropertyMetadata { NotAnimatable = true, Inheritable = true, InheritedFromParent = true });

        #endregion

        #region EasingFunction managed property

        public EasingFunction EasingFunction
        {
            get => (EasingFunction)GetValue(EasingFunctionProperty);
            set => SetValue(EasingFunctionProperty, value);
        }

        public static readonly ManagedProperty EasingFunctionProperty = ManagedProperty.Register(typeof(Storyboard),
            nameof(EasingFunction),
            typeof(EasingFunction),
            new ManagedSimplePropertyMetadata { DefaultValue = EasingFunction.Linear, Inheritable = true, InheritedFromParent = true });

        #endregion

        #region Keyframes managed collection

        public ManagedCollection<BaseKeyframe> Keyframes
        {
            get => (ManagedCollection<BaseKeyframe>)GetValue(KeyframesProperty);
        }

        public static readonly ManagedProperty KeyframesProperty = ManagedProperty.RegisterCollection(typeof(Storyboard),
            nameof(Keyframes),
            typeof(ManagedCollection<BaseKeyframe>));

        #endregion
    }
}
