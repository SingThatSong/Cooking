using Prism.Regions;

namespace Cooking.WPF.Views
{
    /// <summary>
    /// Logic for <see cref="RecipeListView"/>.
    /// </summary>
    public partial class RecipeListView : IRegionMemberLifetime
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RecipeListView"/> class.
        /// </summary>
        public RecipeListView()
        {
            InitializeComponent();
        }

        /// <inheritdoc/>
        public bool KeepAlive => true;
    }
}
