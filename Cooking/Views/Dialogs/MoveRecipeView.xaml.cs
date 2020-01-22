using System.Windows.Input;

namespace Cooking.WPF.Views
{
    /// <summary>
    /// Логика взаимодействия для UserControl1.xaml.
    /// </summary>
    public partial class MoveRecipeView
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MoveRecipeView"/> class.
        /// </summary>
        public MoveRecipeView()
        {
            InitializeComponent();

            // Для того, чтобы окно могло работать с нажатием клавиш на клавиатуре
            // https://stackoverflow.com/a/21352864
            Focusable = true;
            Loaded += (s, e) => Keyboard.Focus(this);
        }
    }
}
