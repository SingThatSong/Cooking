using Avalonia;
using Avalonia.ReactiveUI;

namespace Cooking.Avalonia
{
    /// <summary>
    /// Program class.
    /// </summary>
    internal class Program
    {
        /// <summary>
        /// Initialization code. Don't use any Avalonia, third-party APIs or any
        /// SynchronizationContext-reliant code before AppMain is called: things aren't initialized
        /// yet and stuff might break.
        /// </summary>
        /// <param name="args">Application arguments.</param>
        public static void Main(string[] args) => BuildAvaloniaApp()
                                                    .StartWithClassicDesktopLifetime(args);

        /// <summary>
        /// Avalonia configuration, don't remove; also used by visual designer.
        /// </summary>
        /// <returns>AppBuilder.</returns>
        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .LogToTrace()
                .UseReactiveUI();
    }
}
