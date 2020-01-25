using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Cooking.Avalonia.Views
{
    /// <summary>
    /// Main window.
    /// </summary>
    public class MainWindow : Window
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
        }

        private void InitializeComponent() => AvaloniaXamlLoader.Load(this);
    }
}