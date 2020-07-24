using System.Collections.ObjectModel;

namespace Cooking.Data.Model
{
    /// <summary>
    /// Measurement units hardcoded values.
    /// </summary>
    public class MeasureUnit : Entity
    {
        /// <summary>
        /// Gets or sets measurement unit short name.
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Gets or sets measurement unit full name.
        /// </summary>
        public string? FullName { get; set; }

        /// <summary>
        /// Gets or sets measurement unit full name pluralization in current language.
        /// </summary>
        public string FullNamePluralization { get; set; }
    }
}