using Cooking.WPF.Helpers;
using Cooking.WPF.Views;
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
        public MainWindowViewModel(ILocalizationProvider localizationProvider, ILocalization localization)
        {
            LocalizationProvider = localizationProvider;

            MenuItems = new HamburgerMenuItemCollection()
            {
                new HamburgerMenuIconItem()
                {
                    Label = localization.GetLocalizedString("MainPage"),
                    Icon = new PackIconModern() { Kind = PackIconModernKind.Page },
                    ToolTip = localization.GetLocalizedString("MainPage"),
                    Tag = nameof(WeekView)
                },
                new HamburgerMenuIconItem()
                {
                    Label = localization.GetLocalizedString("Recepies"),
                    Icon = new PackIconModern() { Kind = PackIconModernKind.Food },
                    ToolTip = localization.GetLocalizedString("Recepies"),
                    Tag = nameof(RecipeListView)
                },
                new HamburgerMenuIconItem()
                {
                    Label = localization.GetLocalizedString("Ingredients"),
                    Icon = new PackIconModern() { Kind = PackIconModernKind.PuzzleRound },
                    ToolTip = localization.GetLocalizedString("Ingredients"),
                    Tag = nameof(IngredientListView)
                },
                new HamburgerMenuIconItem()
                {
                    Label = localization.GetLocalizedString("Tags"),
                    Icon = new PackIconModern() { Kind = PackIconModernKind.Tag },
                    ToolTip = localization.GetLocalizedString("Tags"),
                    Tag = nameof(TagListView)
                },
                new HamburgerMenuIconItem()
                {
                    Label = localization.GetLocalizedString("Garnishes"),
                    Icon = new PackIconModern() { Kind = PackIconModernKind.FoodCupcake },
                    ToolTip = localization.GetLocalizedString("Garnishes"),
                    Tag = nameof(GarnishListView)
                },
            };

            SelectedMenuItem = MenuItems[0] as HamburgerMenuIconItem;

            OptionsMenuItems = new HamburgerMenuItemCollection()
            {
                new HamburgerMenuIconItem()
                {
                    Label = localization.GetLocalizedString("Settings"),
                    Icon = new PackIconModern() { Kind = PackIconModernKind.Settings },
                    ToolTip = localization.GetLocalizedString("Settings"),
                    Tag = nameof(SettingsView)
                }
            };
        }

        public ILocalizationProvider LocalizationProvider { get; }

        public HamburgerMenuIconItem? SelectedMenuItem { get; set; }

        public HamburgerMenuItemCollection MenuItems { get; }

        public HamburgerMenuItemCollection OptionsMenuItems { get; }

#pragma warning disable CS0252
        public void SelectMenuItemByViewType(Type type) => SelectedMenuItem = MenuItems.FirstOrDefault(x => x.Tag == type) as HamburgerMenuIconItem;
#pragma warning restore CS0252

    }
}
