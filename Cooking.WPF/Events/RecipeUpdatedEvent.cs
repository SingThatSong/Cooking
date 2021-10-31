using Cooking.WPF.DTO;
using Prism.Events;

namespace Cooking.WPF.Events;

/// <summary>
/// Prism Event fired when recipe updated.
/// </summary>
public class RecipeUpdatedEvent : PubSubEvent<RecipeEdit>
{
}
