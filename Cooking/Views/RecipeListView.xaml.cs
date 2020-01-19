using Prism.Regions;

namespace Cooking.WPF.Views
{
    /// <summary>
    /// Логика взаимодействия для UserControl1.xaml.
    /// </summary>
    public partial class RecipeListView : IRegionMemberLifetime
    {
        public RecipeListView()
        {
            InitializeComponent();
        }

        public bool KeepAlive => true;
    }
}
