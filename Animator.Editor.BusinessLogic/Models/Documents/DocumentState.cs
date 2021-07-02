using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Editor.BusinessLogic.Models.Documents
{
    public class DocumentState
    {
        public DocumentState(int caretOffset, 
            int selectionStart, 
            int selectionLength, 
            double horizontalOffset, 
            double verticalOffset,
            List<FoldSectionState> foldSections)
        {
            CaretOffset = caretOffset;
            SelectionStart = selectionStart;
            SelectionLength = selectionLength;
            HorizontalOffset = horizontalOffset;
            VerticalOffset = verticalOffset;
            FoldSections = foldSections;
        }

        public int CaretOffset { get; }

        public int SelectionStart { get; }

        public int SelectionLength { get; }

        public double HorizontalOffset { get; }

        public double VerticalOffset { get; }

        public List<FoldSectionState> FoldSections { get; }
    }
}
