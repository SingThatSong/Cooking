using Cooking.Pages;

using Cooking.Pages.Ingredients;
using Cooking.Pages.Tags;
using MahApps.Metro.Controls.Dialogs;
using Prism.Ioc;
using Prism.Mvvm;
using Prism.Unity;
using ServiceLayer;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Text;
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
            Trace.Listeners.Add(new TextWriterTraceListener("Log.log"));
            Trace.AutoFlush = true;
            AppDomain.CurrentDomain.UnhandledException += FatalUnhandledException;

            DatabaseService.InitDatabase();
        }

        private const string dateTimeFormat = "dd.MM.yyyy hh:mm";

        private void FatalUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject is Exception exception)
            {
                var exceptionDescription = new StringBuilder();

                exceptionDescription.AppendLine(exception.Message);
                exceptionDescription.AppendLine(exception.StackTrace);

                while (exception.InnerException != null)
                {
                    exception = exception.InnerException;
                    exceptionDescription.AppendLine(exception.Message);
                    exceptionDescription.AppendLine(exception.StackTrace);
                }
                Trace.TraceError($"[{DateTime.Now.ToString(dateTimeFormat, CultureInfo.InvariantCulture)}] {exceptionDescription.ToString()}");
            }
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.Register<MainWindow>();
            containerRegistry.RegisterSingleton<MainWindowViewModel>();

            containerRegistry.RegisterInstance(new DialogUtils(
                                                        Container.Resolve<MainWindowViewModel>(),
                                                        DialogCoordinator.Instance,
                                                        Container.Resolve<IContainerExtension>()
                                                   )
                                              );


            containerRegistry.RegisterForNavigation<WeekSettings>();

            containerRegistry.RegisterForNavigation<ShowGeneratedWeekView>();
            containerRegistry.RegisterForNavigation<MainPage>();
            containerRegistry.RegisterForNavigation<Recepies>();
            containerRegistry.RegisterForNavigation<IngredientsView>();
            containerRegistry.RegisterForNavigation<TagsView>();
            containerRegistry.RegisterForNavigation<GarnishesView>();
        }

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
