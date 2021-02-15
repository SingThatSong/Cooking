using Cooking.Data.Model;
using Cooking.ServiceLayer;
using Cooking.WPF.Controls;
using Cooking.WPF.Views;
using MahApps.Metro.Controls;
using MahApps.Metro.IconPacks;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Linq;

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
        /// <param name="localization">Localization provider dependency.</param>
        /// <param name="tagService">Tag service dependency. Used for menu items generation.</param>
        public MainWindowViewModel(ILocalization localization, CRUDService<Tag> tagService)
        {
            MenuItems = new HamburgerMenuItemCollection()
            {
                new HamburgerMenuIconItem()
                {
                    Label = localization["MainPage"],
                    Icon = new PackIconModern() { Kind = PackIconModernKind.Page },
                    ToolTip = localization["MainPage"],
                    Tag = nameof(WeekView)
                },
                new HamburgerMenuIconItem()
                {
                    Label = localization["Recepies"],
                    Icon = new PackIconModern() { Kind = PackIconModernKind.Food },
                    ToolTip = localization["Recepies"],
                    Tag = nameof(RecipeListView)
                },
                new HamburgerMenuIconItem()
                {
                    Label = localization["Ingredients"],
                    Icon = new PackIconModern() { Kind = PackIconModernKind.PuzzleRound },
                    ToolTip = localization["Ingredients"],
                    Tag = nameof(IngredientListView)
                },
                new HamburgerMenuIconItem()
                {
                    Label = localization["Tags"],
                    Icon = new PackIconModern() { Kind = PackIconModernKind.Tag },
                    ToolTip = localization["Tags"],
                    Tag = nameof(TagListView)
                },
            };

            IEnumerable<TagHamburgerMenuItem> namesForMenuItems = tagService.GetAll(x => x.IsInMenu)
                                                                            .Select(x => new TagHamburgerMenuItem()
                                                                            {
                                                                                Label = x.Name,
                                                                                ToolTip = x.Name,
                                                                                Icon = x.MenuIcon != null ? new PackIconModern() { Kind = (PackIconModernKind)Enum.Parse(typeof(PackIconModernKind), x.MenuIcon) } : null,
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
                    Label = localization["Settings"],
                    Icon = new PackIconModern() { Kind = PackIconModernKind.Settings },
                    ToolTip = localization["Settings"],
                    Tag = nameof(SettingsView)
                }
            };
        }

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
    }
}
