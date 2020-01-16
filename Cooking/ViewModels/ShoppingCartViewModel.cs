using Cooking.WPF.Commands;
using Prism.Regions;
using PropertyChanged;
using ServiceLayer;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Cooking.WPF.Views
{
    [AddINotifyPropertyChangedInterface]
    public partial class ShoppingCartViewModel : INavigationAware
    {
        public DelegateCommand CloseCommand { get; }
        private IRegionNavigationJournal? navigationContext;

        public ShoppingCartViewModel() 
        {
            CloseCommand = new DelegateCommand(Close, canExecute: CanClose);
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

        public ObservableCollection<ShoppingListItem>? List { get; private set; } = new ObservableCollection<ShoppingListItem>();

        public bool IsNavigationTarget(NavigationContext navigationContext) => true;
        public void OnNavigatedFrom(NavigationContext navigationContext) { }
        public void OnNavigatedTo(NavigationContext navigationContext) 
        {
            this.navigationContext = navigationContext.NavigationService.Journal;
            var list = navigationContext.Parameters[nameof(List)] as List<ShoppingListItem>;
            List.AddRange(list);
        }
    }
}