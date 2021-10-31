using System;
using Prism.Events;

namespace Cooking.WPF.Events;

/// <summary>
/// Prism Event fired when recipe deleted.
/// </summary>
public class RecipeDeletedEvent : PubSubEvent<Guid>
{
}
