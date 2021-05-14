using Animator.Editor.BusinessLogic.Models.Dialogs;
using Animator.Editor.Resources;
using Animator.Editor.BusinessLogic.Services.Dialogs;
using Animator.Editor.BusinessLogic.ViewModels.Search;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Animator.Editor.Services.Dialogs
{
    class DialogService : IDialogService
    {
        private readonly Dictionary<ISearchHost, SearchReplaceWindow> searchWindows = new Dictionary<ISearchHost, SearchReplaceWindow>();

        public OpenDialogResult OpenDialog(string filter = null, string title = null, string filename = null)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            if (filename != null)
                dialog.FileName = filename;

            if (filter != null)
                dialog.Filter = filter;
            else
                dialog.Filter = Strings.DefaultFilter;

            if (title != null)
                dialog.Title = title;
            else
                dialog.Title = Strings.DefaultDialogTitle;

            if (dialog.ShowDialog() == true)
                return new OpenDialogResult(true, dialog.FileName);
            else
                return new OpenDialogResult(false, null);
        }

        public SearchReplaceWindowViewModel RequestSearchReplace(ISearchHost searchHost)
        {
            if (!searchWindows.ContainsKey(searchHost))
                searchWindows.Add(searchHost, new SearchReplaceWindow(searchHost));

            return searchWindows[searchHost].ViewModel;
        }

        public SaveDialogResult SaveDialog(string filter = null, string title = null, string filename = null)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            if (filename != null)
                dialog.FileName = filename;

            if (filter != null)
                dialog.Filter = filter;
            else
                dialog.Filter = Strings.DefaultFilter;

            if (title != null)
                dialog.Title = title;
            else
                dialog.Title = Strings.DefaultDialogTitle;

            if (dialog.ShowDialog() == true)
                return new SaveDialogResult(true, dialog.FileName);
            else
                return new SaveDialogResult(false, null);
        }
    }
}
