﻿using Prism.Regions;

namespace Cooking.WPF.Views
{
    /// <summary>
    /// Logic for <see cref="IngredientListView"/>.
    /// </summary>
    public partial class IngredientListView : IRegionMemberLifetime
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IngredientListView"/> class.
        /// </summary>
        public IngredientListView()
        {
            InitializeComponent();
        }

        /// <inheritdoc/>
        public bool KeepAlive => true;
    }
}
