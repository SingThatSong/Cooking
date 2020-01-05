﻿using Cooking.DTO;
using Prism.Events;

namespace Cooking.WPF.Events
{
    public class RecipeUpdatedEvent : PubSubEvent<RecipeEdit> { }
}
