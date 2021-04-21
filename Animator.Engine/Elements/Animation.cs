using Animator.Engine.Base;
using Animator.Engine.Persistence;
using System;
using System.Collections.Generic;
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

        public static readonly ManagedProperty ConfigProperty = ManagedProperty.Register(typeof(Animation),
            nameof(Config),
            typeof(AnimationConfig));

        #endregion

        #region Scenes managed collection

        public List<Scene> Scenes
        {
            get => (List<Scene>)GetValue(ScenesProperty);
        }

        public static readonly ManagedProperty ScenesProperty = ManagedProperty.RegisterCollection(typeof(Animation),
            nameof(Scenes),
            typeof(List<Scene>));

        #endregion
    }
}
