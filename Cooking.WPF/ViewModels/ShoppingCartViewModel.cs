using Cooking.WPF.Commands;
using Prism.Regions;
using PropertyChanged;
using ServiceLayer;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Cooking.WPF.ViewModels
{
    /// <summary>
    /// View model for showing shopping cart.
    /// </summary>
    [AddINotifyPropertyChangedInterface]
    public partial class ShoppingCartViewModel : INavigationAware
    {
        private IRegionNavigationJournal? navigationContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="ShoppingCartViewModel"/> class.
        /// </summary>
        public ShoppingCartViewModel()
        {
            CloseCommand = new DelegateCommand(Close, canExecute: CanClose);
        }

        /// <summary>
        /// Gets command to return to previous view.
        /// </summary>
        public DelegateCommand CloseCommand { get; }

        /// <summary>
        /// Gets shopping cart as list of shopping list groups.
        /// </summary>
        public ObservableCollection<ShoppingListIngredientsGroup>? List { get; private set; } = new ObservableCollection<ShoppingListIngredientsGroup>();

        /// <inheritdoc/>
        public bool IsNavigationTarget(NavigationContext navigationContext) => false;

        /// <inheritdoc/>
        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
        }

        /// <inheritdoc/>
        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            this.navigationContext = navigationContext.NavigationService.Journal;
            var list = navigationContext.Parameters[nameof(List)] as List<ShoppingListIngredientsGroup>;
            List.AddRange(list);
        }

        private bool CanClose()
        {
            if (navigationContext == null)
            {
                return false;
            }
            else
            {
                return navigationContext.CanGoBack;
            }
        }

        private void Close() => navigationContext!.GoBack();
    }
}