namespace UCTemplate.Web.Mvc.Common
{
    #region using

    using System.Globalization;

    #endregion

    public static class StringExtensions
    {
        public static string ToProperCase(this string s)
        {
            var culture = CultureInfo.InvariantCulture.TextInfo;
            return culture.ToTitleCase(culture.ToLower(s));
        }
    }
}