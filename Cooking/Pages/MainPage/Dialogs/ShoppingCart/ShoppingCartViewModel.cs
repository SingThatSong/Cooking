using Cooking.Commands;
using Microsoft.Win32;
using ServiceLayer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Cooking.Pages.Recepies
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