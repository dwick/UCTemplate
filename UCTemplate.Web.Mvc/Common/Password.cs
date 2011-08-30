namespace UCTemplate.Web.Mvc.Common
{
    #region using

    using System;

    #endregion

    public class Password
    {
        /// <summary>
        /// Returns a randomly generated password of the given length.
        /// </summary>
        /// <param name="length">Password length.</param>
        /// <returns>A randomly generated password of the given length.</returns>
        public static string GetRandomPassword(int length)
        {
            const string allowedChars = "abcdefghijkmnopqrstuvwxyzABCDEFGHJKLMNOPQRSTUVWXYZ0123456789!@$?_-";
            var chars = new char[length];
            var rd = new Random();

            for (var i = 0; i < length; i++)
            {
                chars[i] = allowedChars[rd.Next(0, allowedChars.Length)];
            }

            return new string(chars);
        }
    }
}
