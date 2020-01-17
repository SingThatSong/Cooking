using System.Windows.Controls;
using System.Windows.Input;

namespace Cooking.WPF.Views
{
    /// <summary>
    /// Логика взаимодействия для WeekSettings.xaml
    /// </summary>
    public partial class GeneratedWeekView : UserControl
    {
        public GeneratedWeekView()
        {
            InitializeComponent();

            // Для того, чтобы окно могло работать с нажатием клавиш на клавиатуре
            // https://stackoverflow.com/a/21352864
            Focusable = true;
            Loaded += (s, e) => Keyboard.Focus(this);
        }
    }
}
