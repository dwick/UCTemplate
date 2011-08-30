namespace UCTemplate.Web.Mvc.Common
{
    #region using

    using System;
    using System.Collections.Generic;
    using System.DirectoryServices.ActiveDirectory;
    using System.Linq;

    #endregion

    /// <summary>
    /// Active directory extention methods
    /// </summary>
    public static class ActiveDirectoryExtensions
    {
        public static DomainController FindOneDomainController(string domain)
        {
            try
            {
                var context = new DirectoryContext(DirectoryContextType.Domain, domain);
                return DomainController.FindOne(context);

            }
            catch (Exception)
            {
                return null;
            }

        }
        public static IEnumerable<DomainController> FindAllDomainControllers(string domain)
        {
            try
            {
                var context = new DirectoryContext(DirectoryContextType.Domain, domain);
                var searchResult = DomainController.FindAll(context);
                var dcs = new DomainController[searchResult.Count];
                searchResult.CopyTo(dcs, 0);
                return dcs.AsEnumerable();
            }
            catch (Exception)
            {
                return null;
            }

        }
    }
}