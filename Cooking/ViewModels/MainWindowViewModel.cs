using Cooking.WPF.Views;
using Cooking.WPF.Helpers;
using MahApps.Metro.Controls;
using MahApps.Metro.IconPacks;
using PropertyChanged;
using System;
using System.Linq;
using WPFLocalizeExtension.Providers;

namespace Cooking
{
    [AddINotifyPropertyChangedInterface]
    public class MainWindowViewModel
    {
        public MainWindowViewModel(ILocalizationProvider localizationProvider)
        {
            SelectedMenuItem = MenuItems[0] as HamburgerMenuIconItem;
            LocalizationProvider = localizationProvider;
        }

        public ILocalizationProvider LocalizationProvider { get; }

        public HamburgerMenuIconItem? SelectedMenuItem { get; set; }

        public HamburgerMenuItemCollection MenuItems { get; } = new HamburgerMenuItemCollection()
        {
            new HamburgerMenuIconItem()
            {
                Label = "Главная",
                Icon = new PackIconModern() { Kind = PackIconModernKind.Page },
                Tag = nameof(WeekView)
            },
            new HamburgerMenuIconItem()
            {
                Label = "Рецепты",
                Icon = new PackIconModern() { Kind = PackIconModernKind.Food },
                Tag = nameof(RecipeListView)
            },
            new HamburgerMenuIconItem()
            {
                Label = "Ингредиенты",
                Icon = new PackIconModern() { Kind = PackIconModernKind.PuzzleRound },
                Tag = nameof(IngredientListView)
            },
            new HamburgerMenuIconItem()
            {
                Label = "Теги",
                Icon = new PackIconModern() { Kind = PackIconModernKind.Tag },
                Tag = nameof(TagListView)
            },
            new HamburgerMenuIconItem()
            {
                Label = "Гарниры",
                Icon = new PackIconModern() { Kind = PackIconModernKind.FoodCupcake },
                Tag = nameof(GarnishListView)
            },
        };

        public HamburgerMenuItemCollection OptionsMenuItems { get; } = new HamburgerMenuItemCollection()
        {
            new HamburgerMenuIconItem()
            {
                Label = "Настройки",
                Icon = new PackIconModern() { Kind = PackIconModernKind.Settings },
                Tag = nameof(SettingsView)
            }
        };

#pragma warning disable CS0252
        public void SelectMenuItemByViewType(Type type) => SelectedMenuItem = MenuItems.FirstOrDefault(x => x.Tag == type) as HamburgerMenuIconItem;
#pragma warning restore CS0252

    }
}
