using Cooking.Data.Model;
using System.ComponentModel;
using Validar;

namespace Cooking.WPF.DTO
{
    /// <summary>
    /// Sto for ingredient editing.
    /// </summary>
    [InjectValidation]
    public class IngredientEdit : Entity, INotifyPropertyChanged
    {
        /// <inheritdoc/>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Gets or sets ingredient name.
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Gets or sets ingredient type.
        /// </summary>
        public IngredientType? Type { get; set; }
    }
}
