using Prism.Regions;

namespace Cooking.WPF.Views
{
    /// <summary>
    /// Logic for
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

        public bool KeepAlive => true;
    }
}
