namespace UCTemplate.Web.Mvc.Common
{
    #region using

    using System;
    using System.Collections.Generic;

    #endregion

    /// <summary>
    /// IEnumerable extension methods
    /// </summary>
    public static class EnumerableExtensions
    {
        #region public methods

        /// <summary>
        /// Performs an action on a generic list of items.
        /// </summary>
        /// <typeparam name="T">The type of item in the collection.</typeparam>
        /// <param name="items">The list of items.</param>
        /// <param name="action">The action to perform.</param>
        /// <exception cref="ArgumentNullException">Both arguements are required.</exception>
        public static void ForEach<T>(this IEnumerable<T> items, Action<T> action)
        {
            Check.IsNotNull(items, "items");
            Check.IsNotNull(action, "action");

            foreach (var item in items)
            {
                action(item);
            }
        }

        public static IEnumerable<T> Add<T>(this IEnumerable<T> e, T value)
        {
            foreach (var cur in e)
            {
                yield return cur;
            }
            yield return value;
        }

        #endregion
    }
}