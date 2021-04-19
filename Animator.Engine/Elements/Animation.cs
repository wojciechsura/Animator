using Animator.Engine.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Engine.Elements
{
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
    }
}
