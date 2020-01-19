using Prism.Regions;

namespace Cooking.WPF.Views
{
    /// <summary>
    /// Логика взаимодействия для UserControl1.xaml.
    /// </summary>
    public partial class GarnishListView : IRegionMemberLifetime
    {
        public GarnishListView()
        {
            InitializeComponent();
        }

        public bool KeepAlive => true;
    }
}
