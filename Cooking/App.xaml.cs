using AutoMapper;
using Cooking.Data.Context;
using Cooking.Pages;
using Cooking.ServiceLayer;
using Cooking.WPF;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Extensions.Configuration;
using Prism.Ioc;
using Prism.Mvvm;
using Prism.Unity;
using Serilog;
using ServiceLayer;
using System;
using System.Reflection;
using System.Windows;

namespace Cooking
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : PrismApplication
    {
        public App()
        {
            AppDomain.CurrentDomain.UnhandledException += FatalUnhandledException;
            DatabaseService.InitDatabase();
        }

        private void FatalUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var logger = Container.Resolve<ILogger>();
            logger.Error(e.ExceptionObject as Exception, "Critical error");
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            // Register logging
            var logger = new LoggerConfiguration()
                             .MinimumLevel.Information()
                             .WriteTo.Console()
                             .WriteTo.File("Log.txt",
                                 rollingInterval: RollingInterval.Infinite,
                                 rollOnFileSizeLimit: true, 
                                 fileSizeLimitBytes: 1024 * 1024 * 5)
                             .CreateLogger();

            containerRegistry.RegisterInstance<ILogger>(logger);

            IConfigurationRoot configuration = new ConfigurationBuilder()
                                                    .AddJsonFile("appsettings.json", optional: false)
                                                    .Build();

            containerRegistry.RegisterInstance<IConfiguration>(configuration);

            // Register main page and main vm - they are constant
            containerRegistry.Register<MainWindow>();
            containerRegistry.RegisterSingleton<MainWindowViewModel>();

            // Register services
            containerRegistry.RegisterInstance<IMapper>(new Mapper(MapperService.CreateMapper(), Container.Resolve<IContainerExtension>().Resolve));
            containerRegistry.Register<DayService>();
            containerRegistry.Register<GarnishService>();
            containerRegistry.Register<IngredientService>();
            containerRegistry.Register<TagService>();
            containerRegistry.Register<RecipeService>();
            containerRegistry.RegisterSingleton<ImageService>();
            containerRegistry.RegisterSingleton<IContextFactory, ContextFactory>();
            // Dialog service is constant - we have only one window
            containerRegistry.RegisterInstance(new DialogService(
                                                        Container.Resolve<MainWindowViewModel>(),
                                                        DialogCoordinator.Instance,
                                                        Container.Resolve<IContainerExtension>()
                                                   )
                                              );

            // Register pages
            containerRegistry.RegisterForNavigation<WeekSettings>();
            
            containerRegistry.RegisterForNavigation<ShowGeneratedWeekView>();
            containerRegistry.RegisterForNavigation<MainView>();
            containerRegistry.RegisterForNavigation<ShoppingCartView>();
            containerRegistry.RegisterForNavigation<Recepies>();
            containerRegistry.RegisterForNavigation<RecipeView>();
            containerRegistry.RegisterForNavigation<IngredientsView>();
            containerRegistry.RegisterForNavigation<TagsView>();
            containerRegistry.RegisterForNavigation<GarnishesView>();
        }

        /// <summary>
        /// Creating Prism shell (main window)
        /// </summary>
        /// <returns></returns>
        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void ConfigureViewModelLocator()
        {
            base.ConfigureViewModelLocator();

            ViewModelLocationProvider.SetDefaultViewTypeToViewModelTypeResolver((viewType) =>
            {
                var viewName = viewType.FullName;
                var viewAssemblyName = viewType.GetTypeInfo().Assembly.FullName;

                string viewModelName;

                if (viewName != null && viewName.EndsWith("View", StringComparison.OrdinalIgnoreCase))
                {
                    viewModelName = $"{viewName}Model, {viewAssemblyName}";
                }
                else
                {
                    viewModelName = $"{viewName}ViewModel, {viewAssemblyName}";
                }

                return Type.GetType(viewModelName);
            });
        }
    }
}
