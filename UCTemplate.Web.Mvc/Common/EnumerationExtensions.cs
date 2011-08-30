namespace UCTemplate.Web.Mvc.Common
{
    #region using

    using System;

    #endregion

    /// <summary>
    /// Enum extension methods
    /// </summary>
    public static class EnumerationExtensions
    {
        #region private methods

        /// <summary>
        /// Validates the arguments provided.  Used in each static member of this class.
        /// </summary>
        /// <typeparam name="T">The type of the enumeration value.</typeparam>
        /// <param name="enumeration">The enum to operator on.</param>
        /// <param name="value">The value to operate with.</param>
        /// <exception cref="ArgumentNullException">Both parameters are required.</exception>
        /// <exception cref="ArgumentException">Both parameters must be of the same enum type.</exception>
        private static void Validate<T>(Enum enumeration, T value)
        {
            Check.IsNotNull(enumeration, "type");
            Check.IsNotNull(value, "value");

            if (!typeof(T).IsAssignableFrom(enumeration.GetType()))
            {
                throw new ArgumentException(string.Format("'{0}' and '{1}' must be of the same enum type.", enumeration.GetType().Name,
                                                          typeof(T).Name));
            }
        }

        #endregion

        #region public methods

        /// <summary>
        /// Returns whether or not the provided enum contains the specified value.
        /// </summary>
        /// <typeparam name="T">The type of the enumeration value.</typeparam>
        /// <param name="enumeration">The enum to check.</param>
        /// <param name="value">The value to check for.</param>
        /// <exception cref="ArgumentNullException">Both parameters are required.</exception>
        /// <exception cref="ArgumentException">Both parameters must be of the same enum type.</exception>
        /// <returns>Whether or not the provided enum contains the specified value.</returns>
        public static bool Has<T>(this Enum enumeration, T value)
        {
            Validate(enumeration, value);

            return (((int)(object)enumeration & (int)(object)value) == (int)(object)value);
        }

        /// <summary>
        /// Returns whether or not the provided enum is the specified value exactly.
        /// </summary>
        /// <typeparam name="T">The type of the enumeration value.</typeparam>
        /// <param name="enumeration">The enum to check.</param>
        /// <param name="value">The value to check for.</param>
        /// <exception cref="ArgumentNullException">Both parameters are required.</exception>
        /// <exception cref="ArgumentException">Both parameters must be of the same enum type.</exception>
        /// <returns>Whether or not the provided enum is the specified value exactly.</returns>
        public static bool Is<T>(this Enum enumeration, T value)
        {
            Validate(enumeration, value);

            return (int)(object)enumeration == (int)(object)value;
        }

        /// <summary>
        /// Adds an enumeration flag to the provided enum. 
        /// </summary>
        /// <typeparam name="T">The type of the enumeration value.</typeparam>
        /// <param name="enumeration">The enum to add to.</param>
        /// <param name="value">The value to add.</param>
        /// <exception cref="ArgumentNullException">Both parameters are required.</exception>
        /// <exception cref="ArgumentException">Both parameters must be of the same enum type.</exception>
        /// <returns>The resultant enumeration flag with the added value.</returns>
        public static T Add<T>(this Enum enumeration, T value)
        {
            Validate(enumeration, value);

            return (T)(object)(((int)(object)enumeration | (int)(object)value));
        }

        /// <summary>
        /// Removes an enumeration flag from the provided enum. 
        /// </summary>
        /// <typeparam name="T">The type of the enumeration value.</typeparam>
        /// <param name="enumeration">The enum to remove from.</param>
        /// <param name="value">The value to remove.</param>
        /// <exception cref="ArgumentNullException">Both parameters are required.</exception>
        /// <exception cref="ArgumentException">Both parameters must be of the same enum type.</exception>
        /// <returns>The resultant enumeration flag minus the provided value.</returns>
        public static T Remove<T>(this Enum enumeration, T value)
        {
            Validate(enumeration, value);

            return (T)(object)(((int)(object)enumeration & ~(int)(object)value));
        }

        #endregion
    }
}
