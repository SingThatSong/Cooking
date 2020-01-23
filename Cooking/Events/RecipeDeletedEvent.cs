using Prism.Events;
using System;

namespace Cooking.WPF.Events
{
    /// <summary>
    /// Prism Event fired when recipe deleted.
    /// </summary>
    public class RecipeDeletedEvent : PubSubEvent<Guid>
    {
    }
}
