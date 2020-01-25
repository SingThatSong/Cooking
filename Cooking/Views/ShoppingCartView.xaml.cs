﻿using System.Windows.Input;

namespace Cooking.WPF.Views
{
    /// <summary>
    /// Logic for <see cref="ShoppingCartView"/>.
    /// </summary>
    public partial class ShoppingCartView
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ShoppingCartView"/> class.
        /// </summary>
        public ShoppingCartView()
        {
            InitializeComponent();

            // Для того, чтобы окно могло работать с нажатием клавиш на клавиатуре
            // https://stackoverflow.com/a/21352864
            Focusable = true;
            Loaded += (s, e) => Keyboard.Focus(this);
        }
    }
}
