using Animator.Editor.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Editor.BusinessLogic.Models.Configuration.Internal
{
    public class InternalConfig : ConfigItem
    {
        internal const string NAME = "Internal";

        private readonly StoredFiles storedFiles;

        public InternalConfig(BaseItemContainer parent) : base(NAME, parent)
        {
            storedFiles = new StoredFiles(this);
        }

        public StoredFiles StoredFiles => storedFiles;
    }
}
