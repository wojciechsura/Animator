using Animator.Editor.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Editor.BusinessLogic.Models.Configuration.Internal
{
    public class StoredFile : BaseCollectionItem
    {
        internal const string NAME = "StoredFile";

        private readonly ConfigValue<string> filename;
        private readonly ConfigValue<bool> isDirty;
        private readonly ConfigValue<bool> flienameIsVirtual;
        private readonly ConfigValue<string> storedFilename;
        private readonly ConfigValue<string> highlightingName;

        public StoredFile() 
            : base(NAME)
        {
            filename = new ConfigValue<string>("Filename", this);
            isDirty = new ConfigValue<bool>("IsDirty", this);
            flienameIsVirtual = new ConfigValue<bool>("FilenameIsVirtual", this);
            storedFilename = new ConfigValue<string>("StoredFilename", this);
            highlightingName = new ConfigValue<string>("HighlightingName", this);
        }

        public ConfigValue<string> Filename => filename;

        public ConfigValue<bool> IsDirty => isDirty;

        public ConfigValue<bool> FilenameIsVirtual => flienameIsVirtual;

        public ConfigValue<string> StoredFilename => storedFilename;

        public ConfigValue<string> HighlightingName => highlightingName;
    }
}
