﻿using Cooking.Pages;
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

        public HamburgerMenuIconItem? SelectedMenuItem { get; set; }

        public HamburgerMenuItemCollection MenuItems { get; } = new HamburgerMenuItemCollection()
        {
            new HamburgerMenuIconItem()
            {
                Label = "Главная",
                Icon = new PackIconModern() { Kind = PackIconModernKind.Page },
                Tag = typeof(MainView)
            },
            new HamburgerMenuIconItem()
            {
                Label = "Рецепты",
                Icon = new PackIconModern() { Kind = PackIconModernKind.Food },
                Tag = typeof(Recepies)
            },
            new HamburgerMenuIconItem()
            {
                Label = "Ингредиенты",
                Icon = new PackIconModern() { Kind = PackIconModernKind.PuzzleRound },
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
                Icon = new PackIconModern() { Kind = PackIconModernKind.FoodCupcake },
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
