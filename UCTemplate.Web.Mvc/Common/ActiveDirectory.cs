namespace UCTemplate.Web.Mvc.Common
{
    #region using

    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.DirectoryServices.AccountManagement;
    using System.Linq;

    #endregion

    public class ActiveDirectory
    {
        private static volatile IList<PrincipalContext> _connections;
        private static readonly object Sync = new Object();

        public static IList<PrincipalContext> Connections
        {
            get
            {
                if (_connections == null)
                {
                    lock (Sync)
                    {
                        if (_connections == null)
                        {
                            _connections = ActiveDirectoryExtensions.FindAllDomainControllers(ConfigurationManager.AppSettings["Domain"])
                                .Select(x => x.Name).ToList()
                                .Select(x => new PrincipalContext(ContextType.Domain, x)).ToList();

                        }
                    }
                }
                return _connections;
            }
        }

        public static void DisposeConnection(PrincipalContext context)
        {
            lock (Sync)
            {
                context.Dispose();
                Connections.Remove(context);
            }
        }
    }
}