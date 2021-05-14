using Animator.Editor.BusinessLogic.Types.Folding;
using ICSharpCode.AvalonEdit.Highlighting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Animator.Editor.BusinessLogic.Models.Highlighting
{
    public class HighlightingInfo
    {
        public HighlightingInfo(string name, IHighlightingDefinition definition, ImageSource icon, FoldingKind foldingKind, bool hidden)
        {
            Name = name;
            Definition = definition;
            Icon = icon;
            FoldingKind = foldingKind;
            Hidden = hidden;
        }

        public string Name { get; }

        public IHighlightingDefinition Definition { get; }

        public ImageSource Icon { get; }

        public FoldingKind FoldingKind { get; }

        public bool Hidden { get; }

        public string Initial => Name.Substring(0, 1);
    }
}
