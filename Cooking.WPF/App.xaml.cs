using AutoMapper;
using ControlzEx.Theming;
using Cooking.Data.Context;
using Cooking.ServiceLayer;
using Cooking.WPF;
using Cooking.WPF.DTO;
using Cooking.WPF.Services;
using Cooking.WPF.ViewModels;
using Cooking.WPF.Views;
using Fody;
using MahApps.Metro.Controls.Dialogs;
using MaterialDesignThemes.Wpf;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NullGuard;
using Prism.Ioc;
using Prism.Unity;
using Serilog;
using Serilog.Core;
using ServiceLayer;
using SmartFormat;
using SmartFormat.Extensions;
using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using WPFLocalizeExtension.Engine;
using WPFLocalizeExtension.Providers;

// TODO: Dish garnishes select + generate

// TODO: Highlight items like in recipe list everywhere
// TODO: Recipe filtering reserved words localization (and, or, not)
// TODO: Create common view and viewmodel for selecting stuff (replace *SelectViewModel)
// TODO: Add setting to disable suggestion to correct previous week
// TODO: Move this file's parts into different methods. Reason: Too many usings above
// TODO: Add debug console logging to methods and constructors (AOP) ?

// Git-related
// TODO: Setup CI
// TODO: Set autoupgrade https://github.com/ravibpatel/AutoUpdater.NET
// TODO: Use XamlStyler in git hooks
// TODO: Remove usings in git hooks

// Tests-related
// TODO: Tests FTW
// TODO: Use fluent assertions
// TODO: Configure tests coverage
// TODO: Generate lots of data (millions of entries) and test. See: Bogus
// TODO: Debug/ Measure method perfomance (benchmark?)
// TODO: Use static anylizers (PVS Studio)

// Db-related
// TODO: Create installer (Inno setup)
// TODO: Create db migrator for installer (or just Powershell script? )
// TODO: Ensure cascade/set null deletions
// TODO: Ensure db constraints and fks

// New features/plans
// TODO: Count calories for recipe
// TODO: Set calorietype accordingly to counted calories
// TODO: Settings for day recipies (breakfast, supper, etc.)
// TODO: App users
// TODO: Set up failure monitoring
// TODO: Recipe filtering: make Gitlab-like system

// TODO: Use Git(Hub/Lab) issues instead of this list :)

// TODO: Add project documentation (Wiki)

// Things not possible right now
// TODO: Restore Maximize button when ControlzEx fixes it for .NET 5. See https://github.com/ControlzEx/ControlzEx/issues/120
// TODO: Make Mahapps and MaterialDesign work correctly together https://github.com/MaterialDesignInXAML/MaterialDesignInXamlToolkit/wiki/MahAppsMetro-integration. Not available now, See https://github.com/MaterialDesignInXAML/MaterialDesignInXamlToolkit/issues/1896
// TODO: Fix publishing: dotnet publish isnt working, single file isnt working, lib trimming isnt working

// Set Null-check on all func arguments globally
[assembly: NullGuard(ValidationFlags.Arguments)]

// Set ConfigureAwait globally
[assembly: ConfigureAwait(false)]

namespace Cooking
{
    /// <summary>
    /// Logic for App.xaml.
    /// </summary>
    public partial class App : PrismApplication
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="App"/> class.
        /// </summary>
        public App()
        {
            AppDomain.CurrentDomain.UnhandledException += FatalUnhandledException;
        }

        /// <inheritdoc/>
        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            // Register logging
            Logger logger = new LoggerConfiguration()
                             .MinimumLevel.Information()
                             .WriteTo.Console()
                             .WriteTo.File(Consts.LogFilename,
                                           rollingInterval: RollingInterval.Infinite,
                                           rollOnFileSizeLimit: true,
                                           fileSizeLimitBytes: 5 * Consts.Megabyte)
                             .CreateLogger();

            containerRegistry.RegisterInstance<ILogger>(logger);

            IOptions<AppSettings> options = CreateOptions();
            containerRegistry.RegisterInstance(options);

            ThemeManager.Current.ChangeTheme(Current, options.Value.Theme, options.Value.Accent);

            var paletteHelper = new PaletteHelper();
            IBaseTheme baseTheme = MaterialDesignThemes.Wpf.Theme.Dark;
            if (options.Value.Theme == "Light")
            {
                baseTheme = MaterialDesignThemes.Wpf.Theme.Light;
            }

            paletteHelper.SetTheme(MaterialDesignThemes.Wpf.Theme.Create(baseTheme, (Color)ColorConverter.ConvertFromString(options.Value.Accent), (Color)ColorConverter.ConvertFromString(options.Value.Accent)));

            // Register main page and main vm - they are constant
            containerRegistry.Register<MainWindowView>();
            containerRegistry.RegisterSingleton<MainWindowViewModel>();

            // Register services
            containerRegistry.RegisterInstance<IMapper>(new Mapper(MapperService.CreateMapper(), Container.Resolve<IContainerExtension>().Resolve));
            containerRegistry.Register<DayService>();
            containerRegistry.Register<GarnishService>();
            containerRegistry.Register<IngredientService>();
            containerRegistry.Register<TagService>();
            containerRegistry.Register<SettingsService>();
            containerRegistry.Register<RecipeService>();
            containerRegistry.RegisterSingleton<ImageService>();
            containerRegistry.RegisterSingleton<IContextFactory, ContextFactory>();

            // Use instance of provider as singleton for different interfaces
            var jsonProvider = new JsonLocalizationProvider();
            containerRegistry.RegisterInstance<ILocalization>(jsonProvider);
            containerRegistry.RegisterInstance<ILocalizationProvider>(jsonProvider);
            containerRegistry.RegisterInstance<ICurrentCultureProvider>(jsonProvider);

            // If no localization exists or current config is invalid, close application
            EnsureLocalizationProvided();

            // Variables affect pages, so we set them beforehand
            SetStaticVariables();

            // TODO: remove after introducing data migrator
            DatabaseService dbService = Container.Resolve<DatabaseService>();
            dbService.MigrateDatabase();

            // Dialog service is constant - we have only one window
            containerRegistry.RegisterInstance(new DialogService(
                                                        Container.Resolve<MainWindowViewModel>(),
                                                        DialogCoordinator.Instance,
                                                        Container.Resolve<IContainerExtension>(),
                                                        Container.Resolve<ILocalization>()
                                                   )
                                              );

            // Register pages
            containerRegistry.RegisterForNavigation<WeekSettingsView>();
            containerRegistry.RegisterForNavigation<GeneratedWeekView>();
            containerRegistry.RegisterForNavigation<SettingsView>();
            containerRegistry.RegisterForNavigation<WeekView>();
            containerRegistry.RegisterForNavigation<ShoppingCartView>();
            containerRegistry.RegisterForNavigation<RecipeListView>();
            containerRegistry.RegisterForNavigation<RecipeView>();
            containerRegistry.RegisterForNavigation<IngredientListView>();
            containerRegistry.RegisterForNavigation<TagListView>();
            containerRegistry.RegisterForNavigation<GarnishListView>();

            // Register validators
            containerRegistry.Register<IngredientGroupEditValidator>();
            containerRegistry.Register<RecipeEditValidator>();
            containerRegistry.Register<RecipeIngredientEditValidator>();
            containerRegistry.Register<GarnishEditValidator>();
            containerRegistry.Register<TagEditValidator>();
            containerRegistry.Register<IngredientEditValidator>();
        }

        /// <inheritdoc/>
        protected override Window CreateShell()
        {
            MainWindowView mainWindow = Container.Resolve<MainWindowView>();
            IOptions<AppSettings> settings = Container.Resolve<IOptions<AppSettings>>();

            if (settings.Value.IsWindowMaximized)
            {
                mainWindow.WindowState = WindowState.Maximized;
            }

            if (settings.Value.WindowWidth.HasValue)
            {
                mainWindow.Width = settings.Value.WindowWidth.Value;
            }

            if (settings.Value.WindowHeight.HasValue)
            {
                mainWindow.Height = settings.Value.WindowHeight.Value;
            }

            return mainWindow;
        }

        private static IOptions<AppSettings> CreateOptions()
        {
            string? exeFile = Process.GetCurrentProcess().MainModule?.FileName;
            string? directory = Path.GetDirectoryName(exeFile);

            IConfigurationRoot configuration = new ConfigurationBuilder()
                                                    .SetBasePath(directory)
                                                    .AddJsonFile(Consts.AppSettingsFilename, optional: false, reloadOnChange: true)
                                                    .Build();

            var serviceCollection = new ServiceCollection();
            serviceCollection.Configure<AppSettings>(configuration);
            ServiceProvider provider = serviceCollection.BuildServiceProvider();

            return provider.GetRequiredService<IOptions<AppSettings>>();
        }

        private void FatalUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            ILogger logger = Container.Resolve<ILogger>();
            logger.Error(e.ExceptionObject as Exception, "Critical error");
        }

        private void EnsureLocalizationProvided()
        {
            ILocalizationProvider localization = Container.Resolve<ILocalizationProvider>();
            IOptions<AppSettings> configuration = Container.Resolve<IOptions<AppSettings>>();

            if (!localization.AvailableCultures.Any(x => x.Name == configuration.Value.Culture))
            {
                string error = string.Format(CultureInfo.InvariantCulture, Consts.LocalizationNotFound, configuration.Value.Culture);
                MessageBox.Show(error);
                Environment.Exit(0);
            }
        }

        private void SetStaticVariables()
        {
            IOptions<AppSettings> configuration = Container.Resolve<IOptions<AppSettings>>();
            var currentCulture = CultureInfo.GetCultureInfo(configuration.Value.Culture);
            LocalizeDictionary.Instance.Culture = currentCulture;
            Smart.Default.GetFormatterExtension<PluralLocalizationFormatter>().DefaultTwoLetterISOLanguageName = currentCulture.TwoLetterISOLanguageName;

            ILocalization localization = Container.Resolve<ILocalization>();
            TagEdit.Any.Name = localization.GetLocalizedString("Any");
            CalorieTypeSelection.Any.Name = localization.GetLocalizedString("Any");
        }
    }
}
