using Animator.Editor.BusinessLogic.Models.Configuration.Tools.Explorer;
using Animator.Editor.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Editor.BusinessLogic.Models.Configuration.Tools
{
    public class ToolsConfig : ConfigItem
    {
        internal const string NAME = "Tools";

        public ToolsConfig(BaseItemContainer parent) : base(NAME, parent)
        {
            Explorer = new ExplorerConfig(this);
        }

        public ExplorerConfig Explorer { get; }
    }
}
