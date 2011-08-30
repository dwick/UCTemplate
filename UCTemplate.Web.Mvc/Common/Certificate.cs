namespace UCTemplate.Web.Mvc.Common
{
    #region using

    using System;
    using System.Net.Security;
    using System.Security.Cryptography.X509Certificates;

    #endregion

    public class Certificate
    {
        /// <summary>
        /// Always returns true for any cert regardless of who signed it or if it's self-signed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="certificate"></param>
        /// <param name="chain"></param>
        /// <param name="policyErrors"></param>
        /// <returns>Always true for any cert regardless of who signed it or if it's self-signed.</returns>
        public static Boolean AcceptAny(Object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors policyErrors)
        {
            return true;
        }
    }
}