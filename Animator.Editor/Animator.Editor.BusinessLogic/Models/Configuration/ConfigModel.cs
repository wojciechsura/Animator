using Animator.Editor.BusinessLogic.Models.Configuration.Behavior;
using Animator.Editor.BusinessLogic.Models.Configuration.Editor;
using Animator.Editor.BusinessLogic.Models.Configuration.Internal;
using Animator.Editor.BusinessLogic.Models.Configuration.Tools;
using Animator.Editor.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Editor.BusinessLogic.Models.Configuration
{
    public class ConfigModel : ConfigRoot
    {
        internal const string NAME = "DevEditor";

        public ConfigModel() : base(NAME)
        {
            Editor = new EditorConfig(this);
            Behavior = new BehaviorConfig(this);
            Internal = new InternalConfig(this);
            Tools = new ToolsConfig(this);
        }

        public EditorConfig Editor { get; }
        public BehaviorConfig Behavior { get; }
        public InternalConfig Internal { get; }
        public ToolsConfig Tools { get; }
    }
}
