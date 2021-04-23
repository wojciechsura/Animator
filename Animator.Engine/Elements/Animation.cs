using Animator.Engine.Base;
using Animator.Engine.Base.Persistence;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Engine.Elements
{
    [ContentProperty(nameof(Scenes))]
    public class Animation : ManagedObject
    {
        #region Config managed property

        public AnimationConfig Config
        {
            get => (AnimationConfig)GetValue(ConfigProperty);
            set => SetValue(ConfigProperty, value);
        }

        public static readonly ManagedProperty ConfigProperty = ManagedProperty.RegisterReference(typeof(Animation),
            nameof(Config),
            typeof(AnimationConfig));

        #endregion


        #region Scenes managed collection

        public ManagedCollection<Scene> Scenes
        {
            get => (ManagedCollection<Scene>)GetValue(ScenesProperty);
        }

        public static readonly ManagedProperty ScenesProperty = ManagedProperty.RegisterCollection(typeof(Animation),
            nameof(Scenes),
            typeof(ManagedCollection<Scene>));

        #endregion

        #region Animators managed collection

        public ManagedCollection<PropertyAnimator> Animators
        {
            get => (ManagedCollection<PropertyAnimator>)GetValue(AnimatorsProperty);
        }

        public static readonly ManagedProperty AnimatorsProperty = ManagedProperty.RegisterCollection(typeof(Animation),
            nameof(Animators),
            typeof(ManagedCollection<PropertyAnimator>));

        #endregion
    }
}
