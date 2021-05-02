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
    /// <summary>
    /// Root element of the animation.
    /// </summary>
    [ContentProperty(nameof(Scenes))]
    public class Animation : ManagedObject
    {
        #region Config managed property

        /// <summary>
        /// Contains configuration of the animation.
        /// </summary>
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

        /// <summary>
        /// Contains scenes of the animation.
        /// </summary>
        public ManagedCollection<Scene> Scenes
        {
            get => (ManagedCollection<Scene>)GetValue(ScenesProperty);
        }

        public static readonly ManagedProperty ScenesProperty = ManagedProperty.RegisterCollection(typeof(Animation),
            nameof(Scenes),
            typeof(ManagedCollection<Scene>));

        #endregion
    }
}
