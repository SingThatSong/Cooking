using System.Globalization;

namespace Cooking.ServiceLayer
{
    /// <summary>
    /// Interface for culture provider.
    /// </summary>
    public interface ICurrentCultureProvider
    {
        /// <summary>
        /// Gets provide current culture.
        /// </summary>
        CultureInfo CurrentCulture { get; }
    }
}
