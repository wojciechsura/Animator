using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Editor.BusinessLogic.ViewModels.Search
{
    public interface ISearchReplaceWindowAccess
    {
        void ShowAndFocus();
        void ChooseSearchTab();
        void ChooseReplaceTab();
        void Close();
    }
}
