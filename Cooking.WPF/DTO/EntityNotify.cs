using Cooking.Data.Model;
using System.ComponentModel;

namespace Cooking.WPF.DTO
{
    /// <summary>
    /// Entity with interface notification.
    /// </summary>
    public class EntityNotify : Entity, INotifyPropertyChanged
    {
        /// <inheritdoc/>
        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
