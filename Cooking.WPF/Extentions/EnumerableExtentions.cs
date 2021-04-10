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

        /// <summary>
        /// Gets symmetric expection for two IEnumerables. Meaning it gets all elements from first enumerable not preset in second enumerable united with vice versa elements.
        /// </summary>
        /// <typeparam name="T">Collection's item type.</typeparam>
        /// <param name="source">First enumerabke.</param>
        /// <param name="target">Second enumerabke.</param>
        /// <returns>Symmetric expection eenumerable for the two IEnumerables.</returns>
        public static IEnumerable<T> SymmetricException<T>(this IEnumerable<T> source, IEnumerable<T> target)
        {
            return source.Except(target).Union(target.Except(source));
        }

        private static T? RandomElementInternal<T>(this IEnumerable<T> enumerable, Random rand)
        {
            int index = rand.Next(0, enumerable.Count());
            return enumerable.ElementAt(index);
        }
    }
}
