using System;

namespace Cooking.Data.Model.Plan
{
    /// <summary>
    /// Day database entity.
    /// </summary>
    public class Day : Entity
    {
        /// <summary>
        /// Gets or sets foreign key for <see cref="Dinner"/>.
        /// </summary>
        public Guid DinnerID { get; set; }

        /// <summary>
        /// Gets or sets dinner recipe entity.
        /// </summary>
        public virtual Recipe? Dinner { get; set; }

        /// <summary>
        /// Gets or sets foreign key for <see cref="DinnerGarnish"/>.
        /// </summary>
        public Guid? DinnerGarnishID { get; set; }

        /// <summary>
        /// Gets or sets dinner garnish entity.
        /// </summary>
        public virtual Recipe? DinnerGarnish { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether dinner was cooked.
        /// </summary>
        public bool DinnerWasCooked { get; set; }

        /// <summary>
        /// Gets or sets day date.
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Gets or sets day's weekday.
        /// </summary>
        public DayOfWeek DayOfWeek { get; set; }
    }
}