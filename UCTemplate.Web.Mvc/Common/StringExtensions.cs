namespace UCTemplate.Web.Mvc.Common
{
    #region using

    using System;
    using System.Globalization;

    #endregion

    public static class StringExtensions
    {
        public static string ToProperCase(this string s)
        {
            var culture = CultureInfo.InvariantCulture.TextInfo;
            return culture.ToTitleCase(culture.ToLower(s));
        }

        public static bool ContainsInsensitive(this string s, string value)
        {
            return s.IndexOf(value, StringComparison.OrdinalIgnoreCase) >= 0;
        }
    }
}