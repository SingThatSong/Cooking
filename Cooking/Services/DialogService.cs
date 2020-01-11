using Cooking.Pages;
using Cooking.WPF.Helpers;
using MahApps.Metro.Controls.Dialogs;
using Prism.Ioc;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Cooking
{
    public class DialogService
    {
        private readonly IContainerProvider containerProvider;
        private readonly ILocalization localization;

        public object ViewModel { get; }

        public IDialogCoordinator DialogCoordinator { get; }

        public DialogService(object viewModel, 
                             IDialogCoordinator dialogCoordinator, 
                             IContainerExtension containerProvider,
                             ILocalization localization)
        {
            Debug.Assert(viewModel != null);
            Debug.Assert(dialogCoordinator != null);
            Debug.Assert(containerProvider != null);
            Debug.Assert(localization != null);

            DialogCoordinator = dialogCoordinator;
            this.containerProvider = containerProvider;
            this.localization = localization;
            ViewModel = viewModel;
        }

        // Баг mahapps - ожидаем закрытия именно этого окна, а не дочерних
        public virtual async Task ShowAndWaitForClosedAsync(BaseMetroDialog dialog)
        {
            await Application.Current.Dispatcher.Invoke(async () =>
            {
                // Перед отображением запоминаем родителя
                BaseMetroDialog parentDialog = await DialogCoordinator.GetCurrentDialogAsync<BaseMetroDialog>(ViewModel).ConfigureAwait(false);

                await DialogCoordinator.ShowMetroDialogAsync(ViewModel, dialog).ConfigureAwait(false);

                BaseMetroDialog currentDialog;
                do
                {
                    // Unloaded срабатывает в том числе при переключении на дочерние окна
                    // Ждём, пока активным не станет родитель
                    await dialog.WaitUntilUnloadedAsync().ConfigureAwait(false);
                    currentDialog = await DialogCoordinator.GetCurrentDialogAsync<BaseMetroDialog>(ViewModel).ConfigureAwait(false);
                }
                while (currentDialog != parentDialog);
            }).ConfigureAwait(false);
        }

        /// <summary>
        /// Отобразить произвольный View в виде диалогового окна
        /// </summary>
        /// <typeparam name="TDialog">Тип View</typeparam>
        /// <typeparam name="TDialogContent">Тип ViewModel для этого View</typeparam>
        /// <param name="dialogTitle">Заголовок</param>
        /// <param name="content">Объект ViewModel, который будет использован для View</param>
        /// <returns>Объект ViewModel, который может нести значения, введённые пользователем</returns>
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
                CustomDialog dialog = new CustomDialog()
                {
                    Title = title,
                    Content = new TDialog { DataContext = content }
                };

                await ShowAndWaitForClosedAsync(dialog).ConfigureAwait(false);

            }).ConfigureAwait(false);
            return content;
        }

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
                CustomDialog dialog = new CustomDialog()
                {
                    Title = title,
                    Content = new TDialog { DataContext = content }
                };

                await ShowAndWaitForClosedAsync(dialog).ConfigureAwait(true);

                if (content.DialogResultOk)
                {
                    successCallback?.Invoke(content);
                }

            }).ConfigureAwait(false);
        }

        public virtual async Task ShowYesNoDialog(string? title = null, string? content = null, Action? successCallback = null)
        {
            var result = await DialogCoordinator.ShowMessageAsync(
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

        public virtual async Task HideCurrentDialogAsync()
        {
            await Application.Current.Dispatcher.Invoke(async () =>
            {
                var dialog = await DialogCoordinator.GetCurrentDialogAsync<BaseMetroDialog>(ViewModel).ConfigureAwait(false);
                if (dialog != null)
                {
                    await HideDialogAsync(dialog).ConfigureAwait(false);
                }
            }).ConfigureAwait(false);
        }

        public virtual async Task HideDialogAsync(BaseMetroDialog dialog)
        {
            await DialogCoordinator.HideMetroDialogAsync(ViewModel, dialog, new MetroDialogSettings() { AnimateHide = false }).ConfigureAwait(false);
        }
    }
}
