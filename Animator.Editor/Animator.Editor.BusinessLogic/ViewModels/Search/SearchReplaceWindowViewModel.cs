using Animator.Editor.BusinessLogic.Models.Search;
using Animator.Editor.BusinessLogic.Services.Messaging;
using Animator.Editor.BusinessLogic.Types.Search;
using Animator.Editor.BusinessLogic.ViewModels.Base;
using Animator.Editor.Common.Commands;
using Animator.Editor.Common.Conditions;
using Animator.Editor.Common.Tools;
using Animator.Editor.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Animator.Editor.BusinessLogic.ViewModels.Search
{
    public class SearchReplaceWindowViewModel : BaseViewModel
    {
        // Private fields -----------------------------------------------------

        private readonly ISearchHost searchHost;
        private readonly ISearchReplaceWindowAccess access;
        private readonly IMessagingService messagingService;

        private string search;
        private string replace;
        private bool caseSensitive;
        private bool wholeWordsOnly;
        private bool searchBackwards;
        private SearchMode searchMode;

        private bool selectionAvailable;
        private bool replaceAllInSelection;

        // Private methods ----------------------------------------------------

        private Regex GetSearchRegex(string textToFind)
        {
            RegexOptions options = RegexOptions.None;
            if (searchBackwards)
                options |= RegexOptions.RightToLeft;
            if (!caseSensitive)
                options |= RegexOptions.IgnoreCase;

            switch (searchMode)
            {
                case SearchMode.RegularExpressions:
                    return new Regex(textToFind, options);
                case SearchMode.Extended:
                    {
                        string pattern = Regex.Escape(textToFind.Unescape());

                        if (wholeWordsOnly)
                            pattern = "\\b" + pattern + "\\b";
                        return new Regex(pattern, options);
                    }
                case SearchMode.Normal:
                    {
                        string pattern = Regex.Escape(textToFind);
                        if (wholeWordsOnly)
                            pattern = "\\b" + pattern + "\\b";
                        return new Regex(pattern, options);
                    }

                default:
                    throw new InvalidOperationException("Unsupported search mode!");
            }
        }

        private string GetReplaceText(string replace)
        {
            switch (searchMode)
            {
                case SearchMode.Normal:
                    return replace;
                case SearchMode.Extended:
                    return replace.Unescape();
                case SearchMode.RegularExpressions:
                    return replace;
                default:
                    throw new InvalidOperationException("Unsupported search mode!");
            }
        }

        private void DoClose() => access.Close();

        private void DoReplaceAll()
        {
            try
            {
                Regex searchRegex = GetSearchRegex(search);
                string replaceText = GetReplaceText(replace);

                var model = new ReplaceAllModel(searchRegex, replaceText, searchBackwards, replaceAllInSelection);
                searchHost.ReplaceAll(model);
            }
            catch
            {
                messagingService.ShowError(Strings.Message_InvalidSearchPattern);
                return;
            }
        }

        private void DoReplace()
        {
            try
            {
                Regex searchRegex = GetSearchRegex(search);
                string replaceText = GetReplaceText(replace);

                var model = new ReplaceModel(searchRegex, replaceText, searchBackwards);
                searchHost.Replace(model);
            }
            catch
            {
                messagingService.ShowError(Strings.Message_InvalidSearchPattern);
                return;
            }
        }

        private void DoFindNext()
        {
            try
            {
                Regex regex = GetSearchRegex(search);

                var model = new SearchModel(regex, searchBackwards);
                searchHost.FindNext(model);
            }
            catch
            {
                messagingService.ShowError(Strings.Message_InvalidSearchPattern);
                return;
            }
        }

        private void HandleSelectionAvailableChanged(object sender, ValueChangedEventArgs e)
        {
            SelectionAvailable = searchHost.SelectionAvailableCondition.GetValue();
            if (!SelectionAvailable)
                ReplaceAllInSelection = false;
        }

        // Public methods -----------------------------------------------------

        public SearchReplaceWindowViewModel(ISearchHost searchHost, 
            ISearchReplaceWindowAccess access, 
            IMessagingService messagingService)
        {
            this.searchHost = searchHost;
            this.access = access;
            this.messagingService = messagingService;

            search = null;
            replace = null;
            caseSensitive = false;
            wholeWordsOnly = false;
            searchMode = SearchMode.Normal;

            selectionAvailable = searchHost.SelectionAvailableCondition.GetValue();
            searchHost.SelectionAvailableCondition.ValueChanged += HandleSelectionAvailableChanged;

            FindNextCommand = new AppCommand(obj => DoFindNext(), searchHost.CanSearchCondition);
            ReplaceCommand = new AppCommand(obj => DoReplace(), searchHost.CanSearchCondition);
            ReplaceAllCommand = new AppCommand(obj => DoReplaceAll(), searchHost.CanSearchCondition);
            CloseCommand = new AppCommand(obj => DoClose());
        }

        public void ShowSearch()
        {
            access.ChooseSearchTab();
            access.ShowAndFocus();
        }

        public void ShowReplace()
        {
            replaceAllInSelection = searchHost.SelectionAvailableCondition.GetValue();

            access.ChooseReplaceTab();
            access.ShowAndFocus();
        }

        // Public properties --------------------------------------------------

        public string Search
        {
            get => search;
            set => Set(ref search, () => Search, value);
        }

        public string Replace
        {
            get => replace;
            set => Set(ref replace, () => Replace, value);
        }

        public bool CaseSensitive
        {
            get => caseSensitive;
            set => Set(ref caseSensitive, () => CaseSensitive, value);
        }

        public bool WholeWordsOnly
        {
            get => wholeWordsOnly;
            set => Set(ref wholeWordsOnly, () => WholeWordsOnly, value);
        }

        public bool SearchBackwards
        {
            get => searchBackwards;
            set => Set(ref searchBackwards, () => SearchBackwards, value);
        }

        public bool SelectionAvailable
        {
            get => selectionAvailable;
            set => Set(ref selectionAvailable, () => SelectionAvailable, value);
        }

        public bool ReplaceAllInSelection
        {
            get => replaceAllInSelection;
            set => Set(ref replaceAllInSelection, () => ReplaceAllInSelection, value);
        }

        public SearchMode SearchMode
        {
            get => searchMode;
            set => Set(ref searchMode, () => SearchMode, value);
        }

        public ICommand FindNextCommand { get; }
        public ICommand ReplaceCommand { get; }
        public ICommand ReplaceAllCommand { get; }
        public ICommand CloseCommand { get; }
    }
}
