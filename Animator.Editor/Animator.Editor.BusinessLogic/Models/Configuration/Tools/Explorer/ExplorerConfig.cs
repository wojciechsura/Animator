using Animator.Editor.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Editor.BusinessLogic.Models.Configuration.Tools.Explorer
{
    public class ExplorerConfig : ConfigItem
    {
        internal const string NAME = "Explorer";

        public ExplorerConfig(BaseItemContainer parent) : base(NAME, parent)
        {
            FolderTreeHeight = new ConfigValue<double>("FolderTreeHeight", this, 200.0);
        }

        public ConfigValue<double> FolderTreeHeight { get; }
    }
}
