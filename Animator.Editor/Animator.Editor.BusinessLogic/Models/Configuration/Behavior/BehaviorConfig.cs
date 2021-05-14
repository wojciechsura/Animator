using Animator.Editor.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Editor.BusinessLogic.Models.Configuration.Behavior
{
    public class BehaviorConfig : ConfigItem
    {
        internal const string NAME = "Behavior";

        private readonly ConfigValue<Types.Behavior.CloseBehavior> closeBehavior;

        public BehaviorConfig(BaseItemContainer parent) 
            : base(NAME, parent)
        {
            closeBehavior = new ConfigValue<Types.Behavior.CloseBehavior>("CloseBehavior", this, Types.Behavior.CloseBehavior.Standard);
        }

        public ConfigValue<Types.Behavior.CloseBehavior> CloseBehavior => closeBehavior;
    }
}
