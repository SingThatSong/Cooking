using System.Windows.Input;

namespace Cooking.WPF.Views
{
    /// <summary>
    /// Logic for
    /// </summary>
    public partial class IngredientGroupEditView
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IngredientGroupEditView"/> class.
        /// </summary>
        public IngredientGroupEditView()
        {
            InitializeComponent();

            // Для того, чтобы окно могло работать с нажатием клавиш на клавиатуре
            // https://stackoverflow.com/a/21352864
            Focusable = true;
            Loaded += (s, e) => Keyboard.Focus(Focused);
        }
    }
}
