using Animator.Editor.BusinessLogic.Models.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Editor.BusinessLogic.ViewModels.Main
{
    public partial class MainWindowViewModel
    {
        private const int MIN_NAVIGATION_TEXT_LENGTH = 1;

        // Private methods ----------------------------------------------------

        private void DoNavigate()
        {
            NavigationText = String.Empty;
            navigationItems.Clear();

            access.ShowNavigationPopup();
        }

        // Public methods -----------------------------------------------------

        public void PerformNavigationSearch()
        {
            navigationItems.Clear();
            SelectedNavigationItem = null;

            if (navigationText.Length >= MIN_NAVIGATION_TEXT_LENGTH)
            {
                var results = commandRepositoryService.FindMatching(NavigationText)
                    .OrderBy(r => r.Title);

                foreach (var result in results)
                {
                    var model = new CommandNavigationModel(result.Title, imageResources.GetIconByName(result.IconResource), result.Command.CanExecute(null), result.Command);
                    navigationItems.Add(model);

                    SelectedNavigationItem = navigationItems.FirstOrDefault();
                }
            }
        }

        public void NavigationItemChosen()
        {
            if (selectedNavigationItem == null || !selectedNavigationItem.Enabled)
            {
                messagingService.Beep();
            }
            else if (selectedNavigationItem is CommandNavigationModel commandItem)
            {
                access.HideNavigationPopup();
                commandItem.Command.Execute(null);
            }
        }
    }
}
