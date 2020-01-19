using Prism.Regions;

namespace Cooking.WPF.Views
{
    /// <summary>
    /// Логика взаимодействия для UserControl1.xaml.
    /// </summary>
    public partial class IngredientListView : IRegionMemberLifetime
    {
        public IngredientListView()
        {
            InitializeComponent();
        }

        public bool KeepAlive => true;
    }
}
