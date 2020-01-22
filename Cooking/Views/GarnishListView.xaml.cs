using Prism.Regions;

namespace Cooking.WPF.Views
{
    /// <summary>
    /// Логика взаимодействия для UserControl1.xaml.
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
