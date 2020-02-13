using Prism.Events;
using System;

namespace Cooking.WPF.Events
{
    /// <summary>
    /// Prism Event fired when ingredient deleted.
    /// </summary>
    public class IngredientDeletedEvent : PubSubEvent<Guid>
    {
    }
}
