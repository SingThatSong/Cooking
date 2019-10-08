using Cooking.Pages.Garnishes;
using Cooking.Pages.Ingredients;
using Cooking.Pages;
using Cooking.Pages.Recepies;
using Cooking.Pages.Tags;
using MahApps.Metro.Controls;
using MahApps.Metro.IconPacks;
using PropertyChanged;
using System.ComponentModel;

namespace Cooking
{
    [AddINotifyPropertyChangedInterface]
    public class MainWindowViewModel
    {
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
                Tag = new MainPage()
            },
            new HamburgerMenuIconItem()
            {
                Label = "Рецепты",
                Icon = new PackIconModern() { Kind = PackIconModernKind.FoodCupcake },
                Tag = new RecepiesView()
            },
            new HamburgerMenuIconItem()
            {
                Label = "Ингредиенты",
                Icon = new PackIconFontAwesome() { Kind = PackIconFontAwesomeKind.PuzzlePieceSolid },
                Tag = new IngredientsView()
            },
            new HamburgerMenuIconItem()
            {
                Label = "Теги",
                Icon = new PackIconModern() { Kind = PackIconModernKind.Tag },
                Tag = new TagsView()
            },
            new HamburgerMenuIconItem()
            {
                Label = "Гарниры",
                Icon = new PackIconMaterial() { Kind = PackIconMaterialKind.FoodVariant },
                Tag = new GarnishesView(),
                ToolTip = "Гарниры"
            },
        };
    }
}
