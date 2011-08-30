namespace UCTemplate.Web.Mvc.Common
{
    #region using

    using System;

    #endregion

    /// <summary>
    /// Parameter checking
    /// </summary>
    public static class Check
    {
        #region public methods

        /// <summary>
        /// Check's to ensure an object is  not null.
        /// </summary>
        /// <param name="parameter">The object parameter to check</param>
        /// <param name="parameterName">The name of the parameter</param>
        /// <exception cref="ArgumentNullException">Object provided was null.</exception>
        public static void IsNotNull(object parameter, string parameterName)
        {
            if(parameterName == null)
            {
                throw new ArgumentNullException(
                    "parameterName", "parameterName cannot be null.");
            }

            if (parameter == null)
            {
                throw new ArgumentNullException(
                    parameterName, string.Format("{0} cannot be null.", parameterName));
            }
        }

        /// <summary>
        /// Check's to ensure an integer is not negative or 0.
        /// </summary>
        /// <param name="parameter">The int parameter to check</param>
        /// <param name="parameterName">The name of the parameter</param>
        /// <exception cref="ArgumentNullException">Parameter name provided was null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Int provided was negative or zero.</exception>
        public static void IsNotZeroOrNegative(int parameter, string parameterName)
        {
            IsNotNull(parameterName, "parameterName");

            if (parameter <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    parameterName, string.Format("{0} cannot be 0 or negative.", parameterName));
            }
        }

        /// <summary>
        /// Check's to ensure a string is not null or empty.
        /// </summary>
        /// <param name="parameter">The string parameter to check</param>
        /// <param name="parameterName">The name of the parameter</param>
        /// <exception cref="ArgumentNullException">Parameter name provided was null.</exception>
        /// <exception cref="ArgumentException">String provided was null or empty.</exception>
        public static void IsNotNullOrEmpty(string parameter, string parameterName)
        {
            IsNotNull(parameterName, "parameterName");

            if (string.IsNullOrEmpty(parameter))
            {
                throw new ArgumentException(
                    string.Format("{0} cannot be null or empty.", parameterName), parameterName);
            }
        }

        #endregion
    }
}