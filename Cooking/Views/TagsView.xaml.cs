using Prism.Regions;

namespace Cooking.WPF.Views
{
    /// <summary>
    /// Логика взаимодействия для UserControl1.xaml
    /// </summary>
    public partial class TagsView : IRegionMemberLifetime
    {
        public TagsView()
        {
            InitializeComponent();
        }

        public bool KeepAlive => true;
    }
}
