using System.Windows.Input;

namespace Cooking.WPF.Views
{
    /// <summary>
    /// Logic for <see cref="TagSelectView"/>.
    /// </summary>
    public partial class TagSelectView
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TagSelectView"/> class.
        /// </summary>
        public TagSelectView()
        {
            InitializeComponent();

            // Для того, чтобы окно могло работать с нажатием клавиш на клавиатуре
            // https://stackoverflow.com/a/21352864
            Focusable = true;
            Loaded += (s, e) => Keyboard.Focus(this);
        }
    }
}
