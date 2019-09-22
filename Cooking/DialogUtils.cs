using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Cooking
{
    public class DialogUtils
    {
        public IDialogCoordinator DialogCoordinator { get; }
        public object ViewModel { get; }

        public DialogUtils(object viewModel)
        {
            DialogCoordinator = MahApps.Metro.Controls.Dialogs.DialogCoordinator.Instance;
            ViewModel = viewModel;
        }

        // Баг mahapps - ожидаем закрытия именно этого окна, а не дочерних
        public virtual async Task ShowAndWaitForClosedAsync(BaseMetroDialog dialog)
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
        }

        /// <summary>
        /// Отобразить произвольный View в виде диалогового окна
        /// </summary>
        /// <typeparam name="TDialog">Тип View</typeparam>
        /// <typeparam name="TDialogContent">Тип ViewModel для этого View</typeparam>
        /// <param name="dialogTitle">Заголовок</param>
        /// <param name="content">Объект ViewModel, который будет использован для View</param>
        /// <returns>Объект ViewModel, который может нести значения, введённые пользователем</returns>
        public virtual async Task<TDialogContent> ShowCustomMessageAsync<TDialog, TDialogContent>(string title = null, TDialogContent content = null)
            where TDialog : UserControl, new()
            where TDialogContent : class, new()
        {
            content ??= new TDialogContent();

            CustomDialog dialog = new CustomDialog()
            {
                Title = title,
                Content = new TDialog { DataContext = content }
            };

            await ShowAndWaitForClosedAsync(dialog);

            return content;
        }
    }
}
