using Cooking.WPF.DTO;
using Prism.Events;

namespace Cooking.WPF.Events;

/// <summary>
/// Prism Event fired when recipe created.
/// </summary>
public class RecipeCreatedEvent : PubSubEvent<RecipeEdit>
{
}
