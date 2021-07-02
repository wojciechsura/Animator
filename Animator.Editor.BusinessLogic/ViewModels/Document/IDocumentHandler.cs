using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Editor.BusinessLogic.ViewModels.Document
{
    public interface IDocumentHandler : INotifyPropertyChanged
    {
        bool WordWrap { get; }
        bool LineNumbers { get; }

        void RequestClose(DocumentViewModel documentViewModel);
    }
}
