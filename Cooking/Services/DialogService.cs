using Cooking.WPF.Helpers;
using Cooking.WPF.Views;
using MahApps.Metro.Controls.Dialogs;
using Prism.Ioc;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Cooking.WPF
{
    public class DialogService
    {
        private readonly IContainerProvider containerProvider;
        private readonly ILocalization localization;

        public object ViewModel { get; }

        public IDialogCoordinator DialogCoordinator { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DialogService"/> class.
        /// </summary>
        /// <param name="viewModel"></param>
        /// <param name="dialogCoordinator"></param>
        /// <param name="containerProvider"></param>
        /// <param name="localization"></param>
        public DialogService(object viewModel,
                             IDialogCoordinator dialogCoordinator,
                             IContainerExtension containerProvider,
                             ILocalization localization)
        {
            DialogCoordinator = dialogCoordinator;
            this.containerProvider = containerProvider;
            this.localization = localization;
            ViewModel = viewModel;
        }

        // Баг mahapps - ожидаем закрытия именно этого окна, а не дочерних
        /// <summary>
        ///
        /// </summary>
        /// <param name="dialog"></param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        public virtual async Task ShowAndWaitForClosedAsync(BaseMetroDialog dialog)
        {
            await Application.Current.Dispatcher.Invoke(async () =>
            {
                // Перед отображением запоминаем родителя
                BaseMetroDialog parentDialog = await DialogCoordinator.GetCurrentDialogAsync<BaseMetroDialog>(ViewModel);

                await DialogCoordinator.ShowMetroDialogAsync(ViewModel, dialog);

                BaseMetroDialog currentDialog;
                do
                {
                    // Unloaded срабатывает в том числе при переключении на дочерние окна
                    // Ждём, пока активным не станет родитель
                    await dialog.WaitUntilUnloadedAsync();
                    currentDialog = await DialogCoordinator.GetCurrentDialogAsync<BaseMetroDialog>(ViewModel);
                }
                while (currentDialog != parentDialog);
            });
        }

        /// <summary>
        /// Отобразить произвольный View в виде диалогового окна.
        /// </summary>
        /// <typeparam name="TDialog">Тип View.</typeparam>
        /// <typeparam name="TDialogContent">Тип ViewModel для этого View.</typeparam>
        /// <param name="title">Заголовок.</param>
        /// <param name="content">Объект ViewModel, который будет использован для View.</param>
        /// <returns>Объект ViewModel, который может нести значения, введённые пользователем.</returns>
        public virtual async Task<TDialogContent> ShowCustomMessageAsync<TDialog, TDialogContent>(string? title = null, TDialogContent? content = null)
            where TDialog : UserControl, new()
            where TDialogContent : class
        {
            if (content == null)
            {
                content = containerProvider.Resolve<TDialogContent>();
            }

            await Application.Current.Dispatcher.Invoke(async () =>
            {
                var dialog = new CustomDialog()
                {
                    Title = title,
                    Content = new TDialog { DataContext = content }
                };

                await ShowAndWaitForClosedAsync(dialog);
            });
            return content;
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="TDialog"></typeparam>
        /// <typeparam name="TDialogContent"></typeparam>
        /// <param name="title"></param>
        /// <param name="content"></param>
        /// <param name="successCallback"></param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        public virtual async Task ShowOkCancelDialog<TDialog, TDialogContent>(string? title = null, TDialogContent? content = null, Action<TDialogContent>? successCallback = null)
            where TDialog : UserControl, new()
            where TDialogContent : OkCancelViewModel
        {
            if (content == null)
            {
                content = containerProvider.Resolve<TDialogContent>();
            }

            await Application.Current.Dispatcher.Invoke(async () =>
            {
                var dialog = new CustomDialog()
                {
                    Title = title,
                    Content = new TDialog { DataContext = content }
                };

                await ShowAndWaitForClosedAsync(dialog).ConfigureAwait(true);

                if (content.DialogResultOk)
                {
                    successCallback?.Invoke(content);
                }
            });
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="title"></param>
        /// <param name="content"></param>
        /// <param name="successCallback"></param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        public virtual async Task ShowYesNoDialog(string? title = null, string? content = null, Action? successCallback = null)
        {
            MessageDialogResult result = await DialogCoordinator.ShowMessageAsync(
                                                ViewModel,
                                                title,
                                                content,
                                                style: MessageDialogStyle.AffirmativeAndNegative,
                                                settings: new MetroDialogSettings()
                                                {
                                                    AffirmativeButtonText = localization.GetLocalizedString("Yes"),
                                                    NegativeButtonText = localization.GetLocalizedString("No")
                                                }).ConfigureAwait(true);

            if (result == MessageDialogResult.Affirmative)
            {
                successCallback?.Invoke();
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        public virtual async Task HideCurrentDialogAsync()
        {
            await Application.Current.Dispatcher.Invoke(async () =>
            {
                BaseMetroDialog dialog = await DialogCoordinator.GetCurrentDialogAsync<BaseMetroDialog>(ViewModel);
                if (dialog != null)
                {
                    await HideDialogAsync(dialog);
                }
            });
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="dialog"></param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        public virtual async Task HideDialogAsync(BaseMetroDialog dialog)
        {
            await DialogCoordinator.HideMetroDialogAsync(ViewModel, dialog, new MetroDialogSettings() { AnimateHide = false });
        }
    }
}
