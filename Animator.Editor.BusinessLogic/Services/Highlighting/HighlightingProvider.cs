using Animator.Editor.BusinessLogic.Models.Highlighting;
using Animator.Editor.BusinessLogic.Types.Folding;
using Animator.Editor.Resources;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml;

namespace Animator.Editor.BusinessLogic.Services.Highlighting
{
    class HighlightingProvider : IHighlightingProvider, IHighlightingDefinitionReferenceResolver
    {
        // Private fields -----------------------------------------------------

        private const string ResourcePrefix = "Animator.Editor.BusinessLogic.Resources.Highlighting.";

        private readonly List<HighlightingInfo> allHighlightingInfos = new List<HighlightingInfo>();
        private readonly List<HighlightingInfo> visibleHighlightingInfos = new List<HighlightingInfo>();
        private readonly Dictionary<string, HighlightingInfo> highlightingsByExt = new Dictionary<string, HighlightingInfo>(StringComparer.OrdinalIgnoreCase);

        private HighlightingInfo emptyHighlighting;

        // Private methods ----------------------------------------------------

        private Stream OpenResourceStream(string resource)
        {
            return Assembly.GetExecutingAssembly().GetManifestResourceStream(ResourcePrefix + resource);
        }

        private IHighlightingDefinition LoadDefinition(string resourceName)
        {
            XshdSyntaxDefinition xshd;
            using (Stream s = OpenResourceStream(resourceName))
            using (XmlTextReader reader = new XmlTextReader(s))
            {
                xshd = HighlightingLoader.LoadXshd(reader);
            }

            return HighlightingLoader.Load(xshd, this);
        }

        private void RegisterHighlighting(string name, 
            string[] extensions, 
            string resourceName, 
            string iconResourceName, 
            FoldingKind foldingKind,
            bool hidden = false)
        {
            ImageSource icon = null;
            if (iconResourceName != null)
            {
                using (var iconStream = OpenResourceStream(iconResourceName))
                {
                    BitmapImage image = new BitmapImage();
                    image.BeginInit();
                    image.StreamSource = iconStream;
                    image.CacheOption = BitmapCacheOption.OnLoad;
                    image.EndInit();
                    image.Freeze();

                    icon = image;
                }
            }

            var info = new HighlightingInfo(name, LoadDefinition(resourceName), icon, foldingKind, hidden);
            allHighlightingInfos.Add(info);

            if (extensions != null)
                foreach (var ext in extensions)
                    highlightingsByExt.Add(ext, info);
        }

        private void InitializeHighlightings()
        {
            // Special: empty

            emptyHighlighting = new HighlightingInfo(Strings.SyntaxHighlighting_None, null, null, FoldingKind.None, false);
            allHighlightingInfos.Add(emptyHighlighting);

            RegisterHighlighting("XML",
                new[] { ".xml" },
                "xml.xshd",
                "xml.png",
                FoldingKind.Xml);

            // Sort highlightings
            allHighlightingInfos.Sort((i1, i2) => i1.Name.CompareTo(i2.Name));

            // Pick visible highlightings
            visibleHighlightingInfos.AddRange(allHighlightingInfos.Where(hi => !hi.Hidden));
        }

        // IHighlightingDefinitionReferenceResolver implementation ------------

        IHighlightingDefinition IHighlightingDefinitionReferenceResolver.GetDefinition(string name)
        {
            return allHighlightingInfos.First(hi => string.Equals(hi.Definition?.Name, name))
                .Definition;
        }

        // Public methods -----------------------------------------------------

        public HighlightingProvider()
        {
            InitializeHighlightings();            
        }

        public HighlightingInfo GetDefinitionByExtension(string extension)
        {
            if (highlightingsByExt.ContainsKey(extension))
                return highlightingsByExt[extension];

            return emptyHighlighting;
        }

        public HighlightingInfo GetDefinitionByName(string name)
        {
            return allHighlightingInfos
                .FirstOrDefault(hi => String.Equals(hi.Name, name))
                ?? emptyHighlighting;
        }

        // Public properties --------------------------------------------------

        public IReadOnlyList<HighlightingInfo> HighlightingDefinitions => visibleHighlightingInfos;

        public HighlightingInfo EmptyHighlighting => emptyHighlighting;
    }
}
