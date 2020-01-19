using Prism.Events;
using System;

namespace Cooking.WPF.Events
{
    public class RecipeDeletedEvent : PubSubEvent<Guid>
    { }
}
