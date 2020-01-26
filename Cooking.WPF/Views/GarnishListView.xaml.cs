using Prism.Regions;

namespace Cooking.WPF.Views
{
    /// <summary>
    /// Logic for <see cref="GarnishListView"/>.
    /// </summary>
    public partial class GarnishListView : IRegionMemberLifetime
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GarnishListView"/> class.
        /// </summary>
        public GarnishListView()
        {
            InitializeComponent();
        }

        /// <inheritdoc/>
        public bool KeepAlive => true;
    }
}
