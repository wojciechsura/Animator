using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICSharpCode.AvalonEdit.Document;

namespace Animator.Editor.BusinessLogic.ViewModels.Document
{
    public interface IEditorAccess
    {
        void Copy();
        void Cut();
        void Paste();

        (int selStart, int selLength) GetSelection();
        void SetSelection(int selStart, int selLength);
        void ScrollTo(int line, int column);
        string GetSelectedText();
        void BeginChange();
        void EndChange();
        void FocusDocument();
    }
}
