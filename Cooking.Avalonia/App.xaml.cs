using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Cooking.Avalonia.ViewModels;
using Cooking.Avalonia.Views;

// TODO: Make sure there is no russian in code
namespace Cooking.Avalonia
{
    /// <summary>
    /// Application class.
    /// </summary>
    public class App : Application
    {
        /// <inheritdoc/>
        public override void Initialize() => AvaloniaXamlLoader.Load(this);

        /// <inheritdoc/>
        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new MainWindow
                {
                    DataContext = new MainWindowViewModel(),
                };
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}