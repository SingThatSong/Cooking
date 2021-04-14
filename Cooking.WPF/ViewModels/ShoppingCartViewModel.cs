using System.Collections.Generic;
using System.Collections.ObjectModel;
using Cooking.ServiceLayer;
using Prism.Regions;
using PropertyChanged;
using WPF.Commands;

namespace Cooking.WPF.ViewModels
{
    /// <summary>
    /// View model for showing shopping cart.
    /// </summary>
    [AddINotifyPropertyChangedInterface]
    public class ShoppingCartViewModel : INavigationAware
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
        public ObservableCollection<ShoppingListIngredientsGroup>? List { get; } = new ObservableCollection<ShoppingListIngredientsGroup>();

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
            return navigationContext?.CanGoBack == true;
        }

        private void Close() => navigationContext!.GoBack();
    }
}