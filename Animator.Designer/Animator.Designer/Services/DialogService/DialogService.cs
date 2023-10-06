using Animator.Designer.BusinessLogic.Models.AddNamespace;
using Animator.Designer.BusinessLogic.Services.Dialogs;
using Animator.Designer.BusinessLogic.ViewModels.Wrappers;
using Animator.Designer.BusinessLogic.ViewModels.Wrappers.Objects;
using Animator.Designer.Resources;
using Animator.Designer.Windows;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Animator.Designer.Services.DialogService
{
    internal class DialogService : IDialogService
    {
        private readonly Stack<Window> dialogWindows = new();

        private void ActivateLastDialog()
        {
            if (dialogWindows.Any())
                dialogWindows.Peek().Activate();
        }

        private void PopDialog(Window dialog)
        {
            if (dialogWindows.Peek() != dialog)
                throw new InvalidOperationException("Broken dialog window stack mechanism!");

            dialogWindows.Pop();
        }

        private Window GetOwnerWindow()
        {
            return dialogWindows.Any() ? dialogWindows.Peek() : Application.Current.MainWindow;
        }

        public (bool result, string path) ShowOpenDialog(string filter = null, string title = null, string filename = null)
        {
            try
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
                    return (true, dialog.FileName);
                else
                    return (false, null);
            }
            finally
            {
                ActivateLastDialog();
            }
        }

        public (bool result, string path) ShowSaveDialog(string filter = null, string title = null, string filename = null)
        {
            try
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
                    return (true, dialog.FileName);
                else
                    return (false, null);
            }
            finally
            {
                ActivateLastDialog();
            }
        }

        public void ShowExceptionDialog(Exception e)
        {
            var dialog = new ExceptionWindow(e);
            dialog.Owner = GetOwnerWindow();
            dialogWindows.Push(dialog);

            try
            {
                dialog.ShowDialog();
            }
            finally
            {
                PopDialog(dialog);
                ActivateLastDialog();
            }
        }

        public (bool result, string name) ShowNewMacroPropertyDialog(List<string> existingNames)
        {
            MacroPropertyNameWindow dialog = new MacroPropertyNameWindow(existingNames);
            dialog.Owner = GetOwnerWindow();
            dialogWindows.Push(dialog);

            try
            {
                if (dialog.ShowDialog() == true)
                {
                    return (true, dialog.Result);
                }
                else
                {
                    return (false, null);
                }
            }
            finally
            {
                PopDialog(dialog);
                ActivateLastDialog();
            }
        }

        public void ShowMacroPropertyEditor(MacroViewModel macro)
        {
            var dialog = new MacroPropertyEditorWindow(macro);
            dialog.Owner = GetOwnerWindow();
            dialogWindows.Push(dialog);
            try
            {
                dialog.ShowDialog();
            }
            finally
            {
                PopDialog(dialog);
                ActivateLastDialog();
            }
        }

        public (bool result, AddNamespaceResultModel model) ShowAddNamespaceDialog(WrapperContext context)
        {
            var dialog = new AddNamespaceWindow(context);
            dialog.Owner = GetOwnerWindow();
            dialogWindows.Push(dialog);
            try
            {
                var result = dialog.ShowDialog();
                if (result == true)
                    return (true, dialog.Result);
                else
                    return (false, null);
            }
            finally
            {
                PopDialog(dialog);
                ActivateLastDialog();
            }
        }
    }
}
