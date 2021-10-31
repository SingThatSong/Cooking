using System;
using System.Windows;
using System.Windows.Controls;
using Cooking.ServiceLayer;
using Cooking.WPF.ViewModels;
using MahApps.Metro.Controls.Dialogs;
using Prism.Ioc;

namespace Cooking.WPF;

/// <summary>
/// WPF service built on Mahapps' dialog logic.
/// </summary>
public class DialogService
{
    /// <summary>
    /// Container provider for viewmodel's creation.
    /// </summary>
    private readonly IContainerProvider containerProvider;

    /// <summary>
    /// Localization for dialog settings.
    /// </summary>
    private readonly ILocalization localization;

    /// <summary>
    /// Main view model used as context for dialog show.
    /// </summary>
    private readonly object viewModel;

    /// <summary>
    /// Reference to Mahapps' dialog coordinator.
    /// </summary>
    private readonly IDialogCoordinator dialogCoordinator;

    /// <summary>
    /// Initializes a new instance of the <see cref="DialogService"/> class.
    /// </summary>
    /// <param name="viewModel">Main view model of a window.</param>
    /// <param name="dialogCoordinator">Mahapps' dialog coordinator instance.</param>
    /// <param name="containerProvider">Ioc container.</param>
    /// <param name="localization">Localization provider.</param>
    public DialogService(object viewModel,
                         IDialogCoordinator dialogCoordinator,
                         IContainerExtension containerProvider,
                         ILocalization localization)
    {
        this.dialogCoordinator = dialogCoordinator;
        this.containerProvider = containerProvider;
        this.localization = localization;
        this.viewModel = viewModel;
    }

    /// <summary>
    /// Tweak around mahapps' behaviour with dialogs - Unloaded fires when showing child dialogs as well as when hiding dialog itself.
    /// We need to wait til dialog itself is close.
    /// </summary>
    /// <param name="dialog">Dialog to show.</param>
    /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
    public virtual async Task ShowAndWaitForClosedAsync(BaseMetroDialog dialog)
        => await Application.Current.Dispatcher.Invoke(async () =>
        {
                // Remember where we were before showing.
                BaseMetroDialog parentDialog = await dialogCoordinator.GetCurrentDialogAsync<BaseMetroDialog>(viewModel);

            await dialogCoordinator.ShowMetroDialogAsync(viewModel, dialog);

            BaseMetroDialog currentDialog;
            do
            {
                    // Wailt until unload of this dialog (may happen multiple times)
                    await dialog.WaitUntilUnloadedAsync();

                    // Check if current dialog is the one that we remembered
                    currentDialog = await dialogCoordinator.GetCurrentDialogAsync<BaseMetroDialog>(viewModel);
            }
            while (currentDialog != parentDialog);
        });

    /// <summary>
    /// Show any view as dialog.
    /// </summary>
    /// <typeparam name="TDialog">View type.</typeparam>
    /// <typeparam name="TDialogContent">ViewModel type.</typeparam>
    /// <param name="title">Header.</param>
    /// <param name="content">ViewModel object.</param>
    /// <returns>ViewModel, used for a view.</returns>
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
    /// Show any view as dialog.
    /// </summary>
    /// <typeparam name="TDialog">View type.</typeparam>
    /// <typeparam name="TDialogContent">ViewModel type.</typeparam>
    /// <param name="titleToken">Header localization token.</param>
    /// <param name="content">ViewModel object.</param>
    /// <returns>ViewModel, used for a view.</returns>
    public virtual async Task<TDialogContent> ShowCustomLocalizedMessageAsync<TDialog, TDialogContent>(string? titleToken = null, TDialogContent? content = null)
        where TDialog : UserControl, new()
        where TDialogContent : class
        => await ShowCustomMessageAsync<TDialog, TDialogContent>(titleToken != null ? localization[titleToken] : null, content);

    /// <summary>
    /// Show custom dialog with ok/cancel options and run callback on success.
    /// </summary>
    /// <typeparam name="TDialog">Type of dialog.</typeparam>
    /// <typeparam name="TDialogContent">Type of dialog view model.</typeparam>
    /// <param name="title">Title of dialog.</param>
    /// <param name="content">ViewModel. May be null - will be resolved using IoC container.</param>
    /// <param name="successCallback">Callback to be called when user pressed ok.</param>
    /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
    public virtual async Task ShowOkCancelDialogAsync<TDialog, TDialogContent>(string? title = null, TDialogContent? content = null, Action<TDialogContent>? successCallback = null)
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
    /// Show standart yes/no dialog with callback on yes.
    /// </summary>
    /// <param name="titleToken">Dialog title localization token.</param>
    /// <param name="messageToken">Dialog message localization token.</param>
    /// <param name="successCallback">Callback called when user chose yes.</param>
    /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
    public virtual async Task ShowLocalizedYesNoDialogAsync(string? titleToken = null, string? messageToken = null, Action? successCallback = null)
        => await ShowYesNoDialogAsync(titleToken != null ? localization[titleToken] : null,
                                      messageToken != null ? localization[messageToken] : null,
                                      successCallback);

    /// <summary>
    /// Show standart yes/no dialog with callback on yes.
    /// </summary>
    /// <param name="title">Dialog title.</param>
    /// <param name="message">Message for dialog.</param>
    /// <param name="successCallback">Callback called when user chose yes.</param>
    /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
    public virtual async Task ShowYesNoDialogAsync(string? title = null, string? message = null, Action? successCallback = null)
    {
        MessageDialogResult result = await dialogCoordinator.ShowMessageAsync(
                                            viewModel,
                                            title,
                                            message,
                                            style: MessageDialogStyle.AffirmativeAndNegative,
                                            settings: new MetroDialogSettings()
                                            {
                                                AffirmativeButtonText = localization["Yes"],
                                                NegativeButtonText = localization["No"]
                                            }).ConfigureAwait(true);

        if (result == MessageDialogResult.Affirmative)
        {
            successCallback?.Invoke();
        }
    }

    /// <summary>
    /// Hide currently displayed dialog.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
    public virtual async Task HideCurrentDialogAsync()
        => await Application.Current.Dispatcher.Invoke(async () =>
        {
            BaseMetroDialog dialog = await dialogCoordinator.GetCurrentDialogAsync<BaseMetroDialog>(viewModel);
            if (dialog != null)
            {
                await dialogCoordinator.HideMetroDialogAsync(viewModel, dialog, new MetroDialogSettings() { AnimateHide = false });
            }
        });
}
