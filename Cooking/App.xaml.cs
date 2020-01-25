using AutoMapper;
using Cooking.Data.Context;
using Cooking.ServiceLayer;
using Cooking.WPF;
using Cooking.WPF.DTO;
using Cooking.WPF.Services;
using Cooking.WPF.Views;
using Fody;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NullGuard;
using Prism.Ioc;
using Prism.Mvvm;
using Prism.Unity;
using Serilog;
using Serilog.Core;
using ServiceLayer;
using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Windows;
using WPFLocalizeExtension.Engine;
using WPFLocalizeExtension.Providers;

// TODO: Remove Week entity as meta-entity
// TODO: Add centralized configuration edit (now it's only in SettingsViewModel)
// TODO: Add comments to XAML
// TODO: Set Readme.md
// TODO: Set folder names
// TODO: Detect literals = https://stackoverflow.com/questions/29533905/how-to-find-all-the-hardcoded-values-in-a-c-sharp-projectsolution
// TODO: Remove magic literals
// TODO: Add debug console logging to methods and constructors
// TODO: check if .editorconfig uses latest parameters
// TODO: Debug/ Measure method perfomance (benchmark?)
// TODO: Add localization error on startup
// TODO: Move to correct SQLite db, without ID hacks
// TODO: Add project documentation (Wiki)
// TODO: Recipe filtering reserved words localization (and, or, not) ?
// TODO: Replace client recipe filtering with IQuariable filtration
// TODO: Plurals localization
// TODO: Add config to change app theme
// TODO: Move all typesafe enums to tables with localization or to simple enums (IngredientType)
// TODO: Count calories for recipe
// TODO: Set calorietype accordingly to counted calories
// TODO: Replace recipe filtration on client with db filtration
// TODO: Consider using https://github.com/Dresel/MethodCache for caching
// TODO: Add IQueryable as parameter to all selects in CRUDService
// TODO: Consider making IMapper as a dependency for all CRUDServices
// TODO: Make GetCultureSpecificSet method an extention method
// TODO: Add AOP for perfomance monitoring https://github.com/vescon/MethodBoundaryAspect.Fody
// TODO: Make sure there is no russian in the code
// TODO: Move folder name into settings ?
// TODO: Create common view and viewmodel for selecting stuff (replace *SelectViewModel)
// TODO: Set all *Caption bindings to be OneTime

// TODO: Use static anylizers (PVS Studio)
// TODO: Dish garnishes select + generate
// TODO: App users

// Git-related
// TODO: Setup CI
// TODO: Set autoupgrade https://github.com/ravibpatel/AutoUpdater.NET
// TODO: Use XamlStyler in git hooks
// TODO: Remove usings in git hooks

// Tests-related
// TODO: Use fluent assertions
// TODO: Tests FTW
// TODO: Configure tests coverage

// Refactoring
// TODO: Cleanup DTOs
// TODO: Cleanup mappings

// Db-related
// TODO: Create installer (Inno setup)
// TODO: Create db migrator for installer
// TODO: Ensure cascade deletions
// TODO: Ensure effective Db Select (get rid of lazy loading)

// TODO: Replace Extended.WPF.Toolkit when 4.0.0 arrives: https://github.com/xceedsoftware/wpftoolkit/releases

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
                             .WriteTo.File("Log.txt",
                                            rollingInterval: RollingInterval.Infinite,
                                            rollOnFileSizeLimit: true,
                                            fileSizeLimitBytes: 1024 * 1024 * 5)
                             .CreateLogger();

            containerRegistry.RegisterInstance<ILogger>(logger);

            string exeFile = Process.GetCurrentProcess().MainModule.FileName;
            string? directory = Path.GetDirectoryName(exeFile);

            IConfigurationRoot configuration = new ConfigurationBuilder()
                                                    .SetBasePath(directory)
                                                    .AddJsonFile(Consts.SettingsFilename, optional: false)
                                                    .Build();

            containerRegistry.RegisterInstance<IConfiguration>(configuration);

            var serviceCollection = new ServiceCollection();
            serviceCollection.Configure<AppSettings>(configuration);
            ServiceProvider provider = serviceCollection.BuildServiceProvider();
            IOptions<AppSettings> options = provider.GetRequiredService<IOptions<AppSettings>>();

            containerRegistry.RegisterInstance(options);

            // Register main page and main vm - they are constant
            containerRegistry.Register<MainWindowView>();
            containerRegistry.RegisterSingleton<MainWindowViewModel>();

            // Register services
            containerRegistry.RegisterInstance<IMapper>(new Mapper(MapperService.CreateMapper(), Container.Resolve<IContainerExtension>().Resolve));
            containerRegistry.Register<WeekService>();
            containerRegistry.Register<DayService>();
            containerRegistry.Register<GarnishService>();
            containerRegistry.Register<IngredientService>();
            containerRegistry.Register<TagService>();
            containerRegistry.Register<RecipeService>();
            containerRegistry.RegisterSingleton<RecipeFiltrator>();
            containerRegistry.RegisterSingleton<ImageService>();
            containerRegistry.RegisterSingleton<IContextFactory, ContextFactory>();

            // Use instance of provider as singleton for different interfaces
            var jsonProvider = new JsonLocalizationProvider();
            containerRegistry.RegisterInstance<ILocalization>(jsonProvider);
            containerRegistry.RegisterInstance<ILocalizationProvider>(jsonProvider);
            containerRegistry.RegisterInstance<ICurrentCultureProvider>(jsonProvider);

            // Variables affect pages, so we set them beforehand
            SetStaticVariables();

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
        protected override Window CreateShell() => Container.Resolve<MainWindowView>();

        /// <inheritdoc/>
        protected override void ConfigureViewModelLocator()
        {
            base.ConfigureViewModelLocator();

            ViewModelLocationProvider.SetDefaultViewTypeToViewModelTypeResolver((viewType) =>
            {
                string? viewName = viewType.FullName;
                string? viewAssemblyName = viewType.GetTypeInfo().Assembly.FullName;
                string viewModelName = $"{viewName}Model, {viewAssemblyName}";
                return Type.GetType(viewModelName);
            });
        }

        /// <inheritdoc/>
        protected override void OnInitialized()
        {
            base.OnInitialized();

            // TODO: remove after introducing data migrator
            DatabaseService dbService = Container.Resolve<DatabaseService>();
            dbService.MigrateDatabase();
        }

        private void FatalUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            ILogger logger = Container.Resolve<ILogger>();
            logger.Error(e.ExceptionObject as Exception, "Critical error");
        }

        private void SetStaticVariables()
        {
            IOptions<AppSettings> configuration = Container.Resolve<IOptions<AppSettings>>();
            LocalizeDictionary.Instance.Culture = CultureInfo.GetCultureInfo(configuration.Value.Culture);

            ILocalization localization = Container.Resolve<ILocalization>();
            TagEdit.Any.Name = localization.GetLocalizedString("Any");
            CalorieTypeSelection.Any.Name = localization.GetLocalizedString("Any");
        }
    }
}
