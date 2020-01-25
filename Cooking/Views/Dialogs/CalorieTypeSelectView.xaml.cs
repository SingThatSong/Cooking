using System.Windows.Input;

namespace Cooking.WPF.Views
{
    /// <summary>
    /// Logic for <see cref="CalorieTypeSelectView"/>.
    /// </summary>
    public partial class CalorieTypeSelectView
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CalorieTypeSelectView"/> class.
        /// </summary>
        public CalorieTypeSelectView()
        {
            InitializeComponent();

            // Для того, чтобы окно могло работать с нажатием клавиш на клавиатуре
            // https://stackoverflow.com/a/21352864
            Focusable = true;
            Loaded += (s, e) => Keyboard.Focus(this);
        }
    }
}
