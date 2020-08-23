using Cooking.Data.Model;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace Cooking.WPF.DTO
{
    /// <summary>
    /// Entity with interface notification.
    /// </summary>
    public class EntityNotify : Entity, INotifyPropertyChanged
    {
        /// <inheritdoc/>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Raise event with interface notification.
        /// </summary>
        [SuppressMessage("Design", "CA1030", Justification = "Not applicable.")]
        public void RaiseChanged() => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
    }
}
