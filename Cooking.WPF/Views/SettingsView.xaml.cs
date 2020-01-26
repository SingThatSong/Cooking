using Prism.Regions;
using System.Globalization;
using System.Windows.Controls;
using WPFLocalizeExtension.Engine;

namespace Cooking.WPF.Views
{
    /// <summary>
    /// Logic for MainPage.xaml.
    /// </summary>
    public partial class SettingsView : UserControl, IRegionMemberLifetime
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsView"/> class.
        /// </summary>
        public SettingsView()
        {
            InitializeComponent();
        }

        /// <inheritdoc/>
        public bool KeepAlive => true;
    }
}
