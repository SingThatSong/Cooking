using System.ComponentModel;
using Cooking.Data.Model;

namespace Cooking.WPF.DTO;

/// <summary>
/// Entity with interface notification.
/// </summary>
public class EntityNotify : Entity, INotifyPropertyChanged
{
    /// <inheritdoc/>
    public event PropertyChangedEventHandler? PropertyChanged;
}
