using Prism.Regions;

namespace Cooking.Pages
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
