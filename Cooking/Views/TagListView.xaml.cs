using Prism.Regions;

namespace Cooking.WPF.Views
{
    /// <summary>
    /// Logic for
    /// </summary>
    public partial class TagListView : IRegionMemberLifetime
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TagListView"/> class.
        /// </summary>
        public TagListView()
        {
            InitializeComponent();
        }

        public bool KeepAlive => true;
    }
}
