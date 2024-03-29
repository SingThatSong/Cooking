﻿using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Media;
using ControlzEx.Theming;
using Cooking.Data.Context;
using Cooking.ServiceLayer;
using Cooking.WPF;
using Cooking.WPF.DTO;
using Cooking.WPF.Services;
using Cooking.WPF.ViewModels;
using Cooking.WPF.Views;
using MahApps.Metro.Controls.Dialogs;
using MaterialDesignThemes.Wpf;
using Microsoft.Extensions.Configuration;
using Prism.Ioc;
using Prism.Unity;
using Serilog;
using Serilog.Core;
using SmartFormat;
using SmartFormat.Extensions;
using WPF.Commands;
using WPFLocalizeExtension.Engine;
using WPFLocalizeExtension.Providers;

// Tests-related
// TODO: Generate lots of data (millions of entries) and test. See: Bogus
// TODO: Tests FTW
// TODO: UI Tests

// Db-related
// TODO: Create installer (Inno setup) ?
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
// TODO: Move styles to XamlCSS

// TODO: Use Git(Hub/Lab) issues instead of this list :)

// TODO: Add project documentation (Wiki)

// Things not possible right now
// TODO: Make Mahapps and MaterialDesign work correctly together https://github.com/MaterialDesignInXAML/MaterialDesignInXamlToolkit/wiki/MahAppsMetro-integration. Not available now, See https://github.com/MaterialDesignInXAML/MaterialDesignInXamlToolkit/issues/1896
// TODO: Fix publishing: lib trimming isnt working
// TODO: Set autoupgrade https://github.com/Squirrel/Squirrel.Windows Waiting to target .NET 5
namespace Cooking;

/// <summary>
/// Logic for App.xaml.
/// </summary>
public partial class App : PrismApplication
{
    static App()
    {
        // Subscribe to errors in PrismApplication initialization. This is needed because those errors occurs before any IoC initialization
        AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="App"/> class.
    /// </summary>
    public App()
    {
        // Unsubscribe to errors in PrismApplication initialization and move to instance's version
        AppDomain.CurrentDomain.UnhandledException -= CurrentDomain_UnhandledException;
        AppDomain.CurrentDomain.UnhandledException += FatalUnhandledException;
    }

    /// <inheritdoc/>
    protected override void RegisterTypes(IContainerRegistry containerRegistry)
    {
        // Register logging
        Logger logger = new LoggerConfiguration()
                         .MinimumLevel.Information()
                         .WriteTo.Debug()
                         .WriteTo.File(Consts.LogFilename,
                                       rollingInterval: RollingInterval.Infinite,
                                       rollOnFileSizeLimit: true,
                                       fileSizeLimitBytes: 5 * Consts.Megabyte)
                         .CreateLogger();

        containerRegistry.RegisterInstance<ILogger>(logger);

        AppSettings appSettings = CreateOptions();
        containerRegistry.RegisterInstance(appSettings);

        ThemeManager.Current.ChangeTheme(Current, appSettings.Theme, appSettings.Accent);

        var paletteHelper = new PaletteHelper();
        IBaseTheme baseTheme = MaterialDesignThemes.Wpf.Theme.Dark;
        if (appSettings.Theme == "Light")
        {
            baseTheme = MaterialDesignThemes.Wpf.Theme.Light;
        }

        paletteHelper.SetTheme(MaterialDesignThemes.Wpf.Theme.Create(baseTheme,
                                                                    (Color)ColorConverter.ConvertFromString(appSettings.Accent),
                                                                    (Color)ColorConverter.ConvertFromString(appSettings.Accent)));

        // Register main page and main vm - they are constant
        containerRegistry.RegisterSingleton<MainWindowViewModel>();

        containerRegistry.UseAutomapper(Container);

        // Register services
        containerRegistry.RegisterSingleton<ImageService>();
        containerRegistry.Register<IDayService, DayService>();
        containerRegistry.RegisterSingleton<IContextFactory, ContextFactory>();

        // Use instance of provider as singleton for different interfaces
        var jsonProvider = new JsonLocalizationProvider();
        containerRegistry.RegisterInstance<ILocalization>(jsonProvider);
        containerRegistry.RegisterInstance<ILocalizationProvider>(jsonProvider);
        containerRegistry.RegisterInstance<ICurrentCultureProvider>(jsonProvider);

        // If no localization exists or current config is invalid, close application
        EnsureLocalizationProvided();

        // TODO: remove after introducing data migrator
        DatabaseService dbService = Container.Resolve<DatabaseService>();
        dbService.MigrateDatabase();

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
    }

    /// <inheritdoc/>
    protected override Window CreateShell()
    {
        MainWindowView mainWindow = Container.Resolve<MainWindowView>();
        AppSettings settings = Container.Resolve<AppSettings>();

        if (settings.IsWindowMaximized)
        {
            mainWindow.WindowState = WindowState.Maximized;
        }

        if (settings.WindowWidth.HasValue)
        {
            mainWindow.Width = settings.WindowWidth.Value;
        }

        if (settings.WindowHeight.HasValue)
        {
            mainWindow.Height = settings.WindowHeight.Value;
        }

        return mainWindow;
    }

    private static AppSettings CreateOptions()
    {
        string? exeFile = Process.GetCurrentProcess().MainModule?.FileName;
        string? directory = Path.GetDirectoryName(exeFile);

        IConfigurationRoot configuration = new ConfigurationBuilder()
                                                .SetBasePath(directory)
                                                .AddJsonFile(Consts.AppSettingsFilename, optional: false, reloadOnChange: true)
                                                .Build();

        return configuration.Get<AppSettings>();
    }

    private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        var sb = new StringBuilder();

        var error = e.ExceptionObject as Exception;

        while (error != null)
        {
            sb.AppendLine(error.Message)
              .AppendLine(error.StackTrace)
              .AppendLine("--------------------------------------")
              .AppendLine();
            error = error.InnerException;
        }

        File.WriteAllText("error.log", sb.ToString());
    }

    private void FatalUnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        ILogger logger = Container.Resolve<ILogger>();
        logger.Fatal(e.ExceptionObject as Exception, "Critical error");
    }

    private void EnsureLocalizationProvided()
    {
        ILocalizationProvider localization = Container.Resolve<ILocalizationProvider>();
        AppSettings configuration = Container.Resolve<AppSettings>();

        if (!localization.AvailableCultures.Any(x => x.Name == configuration.Culture))
        {
            string error = string.Format(CultureInfo.InvariantCulture, Consts.LocalizationNotFound, configuration.Culture);
            MessageBox.Show(error);
            Environment.Exit(0);
        }
    }

    private void SetStaticVariables()
    {
        AppSettings configuration = Container.Resolve<AppSettings>();
        LocalizeDictionary.Instance.Culture = CultureInfo.GetCultureInfo(configuration.Culture);

        LocalizeDictionary.Instance.DefaultProvider = Container.Resolve<ILocalizationProvider>();
        LocalizeDictionary.Instance.DisableCache = false;
        LocalizeDictionary.Instance.IncludeInvariantCulture = false;

        ILocalization localization = Container.Resolve<ILocalization>();
        TagEdit.Any.Name = localization["Any"];
        CalorieTypeSelection.Any.Name = localization["Any"];

        DayService dayService = Container.Resolve<DayService>();
        dayService.InitCache();

        DelegateCommandBase.GlobalExceptionHandler = ex =>
        {
            ILogger logger = Container.Resolve<ILogger>();
            logger.Error(ex, "Critical error");
            return true;
        };
    }
}
