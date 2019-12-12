using ServiceLayer;
using System.Collections.Generic;

namespace Cooking.Pages
{
    public partial class ShoppingCartViewModel : DialogViewModel
    {
        public ShoppingCartViewModel() { }

        public ShoppingCartViewModel(List<ShoppongListItem> list)
        {
            List = list;
        }

        public List<ShoppongListItem>? List { get; }
    }
}