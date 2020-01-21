using System;
using System.Collections.Generic;

namespace Cooking.Data.Model.Plan
{
    /// <summary>
    /// Week database entity.
    /// </summary>
    public class Week : Entity
    {
        /// <summary>
        /// Gets or sets week start day.
        /// </summary>
        public DateTime Start { get; set; }

        /// <summary>
        /// Gets or sets week end day.
        /// </summary>
        public DateTime End { get; set; }

        /// <summary>
        /// Gets or sets week days.
        /// </summary>
        public virtual List<Day>? Days { get; set; }
    }
}
