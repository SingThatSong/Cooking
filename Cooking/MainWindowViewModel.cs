using Cooking.Pages;
using Cooking.WPF.Helpers;
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
        public MainWindowViewModel()
        {
            SelectedMenuItem = MenuItems[0] as HamburgerMenuIconItem;
        }

        public JsonLocalizationProvider LocalizationProvider { get; } = new JsonLocalizationProvider();

        public HamburgerMenuIconItem? SelectedMenuItem { get; set; }

        public HamburgerMenuItemCollection MenuItems { get; } = new HamburgerMenuItemCollection()
        {
            new HamburgerMenuIconItem()
            {
                Label = "Главная",
                Icon = new PackIconModern() { Kind = PackIconModernKind.Page },
                Tag = nameof(MainView)
            },
            new HamburgerMenuIconItem()
            {
                Label = "Рецепты",
                Icon = new PackIconModern() { Kind = PackIconModernKind.Food },
                Tag = nameof(Recepies)
            },
            new HamburgerMenuIconItem()
            {
                Label = "Ингредиенты",
                Icon = new PackIconModern() { Kind = PackIconModernKind.PuzzleRound },
                Tag = nameof(IngredientsView)
            },
            new HamburgerMenuIconItem()
            {
                Label = "Теги",
                Icon = new PackIconModern() { Kind = PackIconModernKind.Tag },
                Tag = nameof(TagsView)
            },
            new HamburgerMenuIconItem()
            {
                Label = "Гарниры",
                Icon = new PackIconModern() { Kind = PackIconModernKind.FoodCupcake },
                Tag = nameof(GarnishesView)
            },
        };

        public HamburgerMenuItemCollection OptionsMenuItems { get; } = new HamburgerMenuItemCollection()
        {
            new HamburgerMenuIconItem()
            {
                Label = "Настройки",
                Icon = new PackIconModern() { Kind = PackIconModernKind.Settings },
                Tag = nameof(Settings)
            }
        };

        public void SelectMenuItemByViewType(Type type)
        {
#pragma warning disable CS0252
            SelectedMenuItem = MenuItems.FirstOrDefault(x => x.Tag == type) as HamburgerMenuIconItem;
#pragma warning restore CS0252
        }
    }
}
