using System.Windows.Controls;
using System.Windows.Input;

namespace Cooking.Pages.Dialogs
{
    /// <summary>
    /// Логика взаимодействия для WeekSettings.xaml
    /// </summary>
    public partial class WeekSettings : UserControl
    {
        public WeekSettings()
        {
            InitializeComponent();

            // Для того, чтобы окно могло работать с нажатием клавиш на клавиатуре
            // https://stackoverflow.com/a/21352864
            Focusable = true;
            Loaded += (s, e) => Keyboard.Focus(this);
        }
    }
}
