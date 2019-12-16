using Prism.Regions;

namespace Cooking.Pages
{
    /// <summary>
    /// Логика взаимодействия для UserControl1.xaml
    /// </summary>
    public partial class IngredientsView : IRegionMemberLifetime
    {
        public IngredientsView()
        {
            InitializeComponent();
        }

        public bool KeepAlive => true;
    }
}
