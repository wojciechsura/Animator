using ICSharpCode.AvalonEdit.Folding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Editor.BusinessLogic.Models.Documents
{
    public class FoldSectionState
    {
        public FoldSectionState(FoldingSection foldingSection)
        {
            StartOffset = foldingSection.StartOffset;
            EndOffset = foldingSection.EndOffset;
            IsFolded = foldingSection.IsFolded;
        }

        public FoldSectionState(int startOffset, int endOffset, bool isFolded)
        {
            StartOffset = startOffset;
            EndOffset = endOffset;
            IsFolded = isFolded;
        }

        public int StartOffset { get; }
        public int EndOffset { get; }
        public bool IsFolded { get; }
    }
}
