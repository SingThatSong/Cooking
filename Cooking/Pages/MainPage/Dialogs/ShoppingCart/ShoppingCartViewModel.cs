using Cooking.Commands;
using Prism.Regions;
using PropertyChanged;
using ServiceLayer;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Cooking.Pages
{
    [AddINotifyPropertyChangedInterface]
    public partial class ShoppingCartViewModel : INavigationAware
    {
        public DelegateCommand CloseCommand { get; }
        private NavigationContext? navigationContext;

        public ShoppingCartViewModel() 
        {
            CloseCommand = new DelegateCommand(Close);
        }

        private void Close()
        {
            navigationContext.NavigationService.Journal.GoBack();
        }

        public ObservableCollection<ShoppingListItem>? List { get; private set; } = new ObservableCollection<ShoppingListItem>();

        public bool IsNavigationTarget(NavigationContext navigationContext) => true;
        public void OnNavigatedFrom(NavigationContext navigationContext) { }
        public void OnNavigatedTo(NavigationContext navigationContext) 
        {
            this.navigationContext = navigationContext;
            var list = navigationContext.Parameters[nameof(List)] as List<ShoppingListItem>;
            List.AddRange(list);
        }
    }
}