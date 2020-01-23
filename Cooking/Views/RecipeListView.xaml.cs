using Prism.Regions;

namespace Cooking.WPF.Views
{
    /// <summary>
    /// Logic for
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

        public bool KeepAlive => true;
    }
}
