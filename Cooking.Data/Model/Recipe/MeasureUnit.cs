using System;
using System.Collections.ObjectModel;

namespace Cooking.Data.Model
{
    /// <summary>
    /// Measurement units hardcoded values.
    /// </summary>
    public class MeasureUnit : Entity
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MeasureUnit"/> class.
        /// </summary>
        public MeasureUnit()
        {
            Name = string.Empty;
            FullNamePluralization = string.Empty;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MeasureUnit"/> class.
        /// </summary>
        /// <param name="name">Name of a measure unit.</param>
        /// <param name="fullNamePluralization">Pluralizations for measure unit.</param>
        public MeasureUnit(string name, string fullNamePluralization)
        {
            Name = name;
            FullNamePluralization = fullNamePluralization;
        }

        /// <summary>
        /// Gets or sets measurement unit short name.
        /// </summary>
        public string Name { get; set; }

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