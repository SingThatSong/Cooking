using Prism.Regions;

namespace Cooking.WPF.Views
{
    /// <summary>
    /// Логика взаимодействия для UserControl1.xaml.
    /// </summary>
    public partial class RecipeListView : IRegionMemberLifetime
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RecipeListView"/> class.
        /// </summary>
        public RecipeListView()
        {
            InitializeComponent();
        }

        public bool KeepAlive => true;
    }
}
