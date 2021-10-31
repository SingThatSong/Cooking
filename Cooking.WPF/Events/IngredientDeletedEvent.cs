using System;
using Prism.Events;

namespace Cooking.WPF.Events;

/// <summary>
/// Prism Event fired when ingredient deleted.
/// </summary>
public class IngredientDeletedEvent : PubSubEvent<Guid>
{
}
