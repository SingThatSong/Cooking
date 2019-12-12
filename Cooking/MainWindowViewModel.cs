using Cooking.Pages;

using Cooking.Pages.Ingredients;
using Cooking.Pages.Tags;
using MahApps.Metro.Controls;
using MahApps.Metro.IconPacks;
using PropertyChanged;
using System;
using System.Linq;

namespace Cooking
{
    [AddINotifyPropertyChangedInterface]
    public class MainWindowViewModel
    {
        private Guid t = Guid.NewGuid();
        public MainWindowViewModel()
        {
            SelectedMenuItem = MenuItems[0] as HamburgerMenuIconItem;
        }

        public HamburgerMenuIconItem? SelectedMenuItem { get; set; }

        public HamburgerMenuItemCollection MenuItems { get; } = new HamburgerMenuItemCollection()
        {
            new HamburgerMenuIconItem()
            {
                Label = "Главная",
                Icon = new PackIconModern() { Kind = PackIconModernKind.Page },
                Tag = typeof(MainPage)
            },
            new HamburgerMenuIconItem()
            {
                Label = "Рецепты",
                Icon = new PackIconModern() { Kind = PackIconModernKind.FoodCupcake },
                Tag = typeof(Recepies)
            },
            new HamburgerMenuIconItem()
            {
                Label = "Ингредиенты",
                Icon = new PackIconFontAwesome() { Kind = PackIconFontAwesomeKind.PuzzlePieceSolid },
                Tag = typeof(IngredientsView)
            },
            new HamburgerMenuIconItem()
            {
                Label = "Теги",
                Icon = new PackIconModern() { Kind = PackIconModernKind.Tag },
                Tag = typeof(TagsView)
            },
            new HamburgerMenuIconItem()
            {
                Label = "Гарниры",
                Icon = new PackIconMaterial() { Kind = PackIconMaterialKind.FoodVariant },
                Tag = typeof(GarnishesView)
            },
        };

        public void SelectMenuItemByViewType(Type type)
        {
#pragma warning disable CS0252
            SelectedMenuItem = MenuItems.FirstOrDefault(x => x.Tag == type) as HamburgerMenuIconItem;
#pragma warning restore CS0252
        }
    }
}
