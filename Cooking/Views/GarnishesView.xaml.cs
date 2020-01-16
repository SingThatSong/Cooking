using Prism.Regions;

namespace Cooking.WPF.Views
{
    /// <summary>
    /// Логика взаимодействия для UserControl1.xaml
    /// </summary>
    public partial class GarnishesView : IRegionMemberLifetime
    {
        public GarnishesView()
        {
            InitializeComponent();
        }

        public bool KeepAlive => true;
    }
}
