using Prism.Regions;

namespace Cooking.WPF.Views
{
    /// <summary>
    /// Логика взаимодействия для UserControl1.xaml.
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
