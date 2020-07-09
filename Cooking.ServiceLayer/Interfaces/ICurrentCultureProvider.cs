using System.Globalization;

namespace Cooking.ServiceLayer
{
    /// <summary>
    /// Interface for culture provider.
    /// </summary>
    public interface ICurrentCultureProvider
    {
        /// <summary>
        /// Gets current culture.
        /// </summary>
        CultureInfo CurrentCulture { get; }
    }
}
