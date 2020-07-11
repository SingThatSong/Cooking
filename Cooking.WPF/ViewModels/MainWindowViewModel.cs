using Cooking.ServiceLayer;
using Cooking.WPF.Controls;
using Cooking.WPF.Services;
using Cooking.WPF.Views;
using MahApps.Metro.Controls;
using MahApps.Metro.IconPacks;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Linq;
using WPFLocalizeExtension.Providers;

namespace Cooking.WPF.ViewModels
{
    /// <summary>
    /// View model for main window.
    /// </summary>
    [AddINotifyPropertyChangedInterface]
    public class MainWindowViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindowViewModel"/> class.
        /// </summary>
        /// <param name="localizationProvider">Localization provider for WPFLocalizeExtension.</param>
        /// <param name="localization">Localization provider dependency.</param>
        /// <param name="tagService">Tag service dependency. Used for menu items generation.</param>
        public MainWindowViewModel(ILocalizationProvider localizationProvider, ILocalization localization, TagService tagService)
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

            IEnumerable<TagHamburgerMenuItem> namesForMenuItems = tagService.GetMenuTags()
                                                                            .Select(x => new TagHamburgerMenuItem()
                                                                            {
                                                                                Label = x.Name,
                                                                                ToolTip = x.Name,
                                                                                Icon = new PackIconModern() { Kind = (PackIconModernKind)Enum.Parse(typeof(PackIconModernKind), x.MenuIcon!) },
                                                                                Tag = nameof(RecipeListView)
                                                                            });

            foreach (HamburgerMenuItem menuItem in namesForMenuItems)
            {
                MenuItems.Insert(2, menuItem);
            }

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

        /// <summary>
        /// Gets localization provider for WPFLocalizeExtension.
        /// </summary>
        public ILocalizationProvider LocalizationProvider { get; }

        /// <summary>
        /// Gets or sets selected menu item.
        /// </summary>
        public HamburgerMenuIconItem? SelectedMenuItem { get; set; }

        /// <summary>
        /// Gets all menu items.
        /// </summary>
        public HamburgerMenuItemCollection MenuItems { get; }

        /// <summary>
        /// Gets all options.
        /// </summary>
        public HamburgerMenuItemCollection OptionsMenuItems { get; }

        /// <summary>
        /// Update Selected menu item.
        /// </summary>
        /// <param name="type">Type of view to activate menu item.</param>
        public void SelectMenuItemByViewType(string text)
        {
            SelectedMenuItem = MenuItems.OfType<HamburgerMenuIconItem>().FirstOrDefault(x => x.Label == text);
        }
    }
}
