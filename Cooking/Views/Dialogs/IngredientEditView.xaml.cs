using System.Windows.Input;

namespace Cooking.WPF.Views
{
    /// <summary>
    /// Logic for <see cref="IngredientEditView"/>.
    /// </summary>
    public partial class IngredientEditView
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IngredientEditView"/> class.
        /// </summary>
        public IngredientEditView()
        {
            InitializeComponent();

            // Для того, чтобы окно могло работать с нажатием клавиш на клавиатуре
            // https://stackoverflow.com/a/21352864
            Focusable = true;
            Loaded += (s, e) => Keyboard.Focus(Focused);
        }
    }
}
