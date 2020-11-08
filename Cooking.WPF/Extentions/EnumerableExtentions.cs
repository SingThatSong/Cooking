using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cooking.WPF.Services
{
    /// <summary>
    /// Extentions for <see cref="IRegionManager"/>.
    /// </summary>
    public static class EnumerableExtentions
    {
        /// <summary>
        /// Get random element from a collection.
        /// </summary>
        /// <typeparam name="T">Collection's item type.</typeparam>
        /// <param name="enumerable">Enumerable where to search for item.</param>
        /// <returns>Random item from collection.</returns>
        public static T? RandomElement<T>(this IEnumerable<T> enumerable)
        {
            if (!enumerable.Any())
            {
                return default;
            }

            return enumerable.RandomElementInternal(new Random());
        }

        /// <summary>
        /// Get random element from a collection.
        /// </summary>
        /// <typeparam name="T">Collection's item type.</typeparam>
        /// <param name="enumerable">Enumerable where to search for item.</param>
        /// <param name="rand">Instance of random class as optimization.</param>
        /// <returns>Random item from collection.</returns>
        public static T? RandomElementUsing<T>(this IEnumerable<T> enumerable, Random rand)
        {
            if (!enumerable.Any())
            {
                return default;
            }

            return enumerable.RandomElementInternal(rand);
        }

        private static T? RandomElementInternal<T>(this IEnumerable<T> enumerable, Random rand)
        {
            int index = rand.Next(0, enumerable.Count());
            return enumerable.ElementAt(index);
        }
    }
}
