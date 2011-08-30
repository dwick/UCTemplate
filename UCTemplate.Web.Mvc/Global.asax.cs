namespace UCTemplate.Web.Mvc
{
    #region using

    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Diagnostics;
    using System.DirectoryServices.AccountManagement;
    using System.Linq;
    using System.Net;
    using System.Web;
    using System.Web.Caching;
    using System.Web.Mvc;
    using System.Web.Routing;

    using Elmah;
    using log4net;
    using log4net.Config;
    using log4net.Core;
    using MvcMiniProfiler;
    using MvcMiniProfiler.MVCHelpers;

    using Common;
    using Helpers.Routing;
    using Infrastructure.Logging;
    using Infrastructure.Tasks;
    using Mapping;

    #endregion

    public class MvcApplication : HttpApplication
    {
        private const string TASK_QUEUE_CACHE_ITEM_KEY = "Tasks";

        #region private fields

        private static readonly ILog Log = LogManager.GetLogger(typeof(MvcApplication));
        private static volatile IList<PrincipalContext> _domainConnections;
        private static readonly object Sync = new Object();
        private static readonly string TaskQueueCacheRefresherPageUrl = ConfigurationManager.AppSettings["ProductionUrl"] + "/RefreshTaskQueueCache.aspx";

        #endregion

        public static readonly List<ITask> TaskQueue = new List<ITask>();

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.IgnoreRoute("{*favicon}", new { favicon = @"(.*/)?favicon.ico(/.*)?" });

            routes.MapRouteLowerCase(
                "Home",
                "{action}",
                new { controller = "Home", action = "Index" },
                new { action = @"(?i:guide|contact|index)" }
                );

            routes.MapRouteLowerCase(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "Home", action = "Index", id = UrlParameter.Optional } // Parameter defaults
            );

        }

        /// <summary>
        /// Filters out 404s/debugging from the error emails.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void ErrorMail_Filtering(object sender, ExceptionFilterEventArgs e)
        {
            var httpException = e.Exception as HttpException;
            if (httpException != null && httpException.GetHttpCode() == 404 || Debugger.IsAttached)
            {
                e.Dismiss();
            }
        }

        protected void Application_Start()
        {
            ViewEngines.Engines.Clear();
            ViewEngines.Engines.Add(new ProfilingViewEngine(new RazorViewEngine()));

            AreaRegistration.RegisterAllAreas();

            GlobalFilters.Filters.Add(new ProfilingActionFilter());
            GlobalFilters.Filters.Add(new HandleErrorAttribute());

            BasicConfigurator.Configure(
                Log4NetConfiguration.GetSqlLogAppender(
                    ConfigurationManager.ConnectionStrings["UCTemplate"].ToString(),
                    "Logs", Level.All));

            Log.Info("Application started.");

            AutoMapperConfiguration.Configure();

            RegisterRoutes(RouteTable.Routes);
#if !DEBUG
            RegisterTaskQueueCacheEntry();
#endif
        }

        protected void Application_BeginRequest()
        {
            if (HttpContext.Current.Request.Url.ToString() == TaskQueueCacheRefresherPageUrl)
            {
                RegisterTaskQueueCacheEntry();
            }

            if (Request.IsLocal) { MiniProfiler.Start(); }
        }

        protected void Application_EndRequest()
        {
            MiniProfiler.Stop();
        }

       
        public static IList<PrincipalContext> DomainConnections
        {
            get
            {
                if (_domainConnections == null)
                {
                    lock (Sync)
                    {
                        if (_domainConnections == null)
                        {
                            _domainConnections = ActiveDirectoryExtensions.FindAllDomainControllers(ConfigurationManager.AppSettings["Domain"])
                                .Select(x => x.Name).ToList()
                                .Select(x => new PrincipalContext(ContextType.Domain, x)).ToList();

                        }
                    }
                }
                return _domainConnections;
            }
        }

        public static void DisposeConnection(PrincipalContext context)
        {
            lock (Sync)
            {
                context.Dispose();
                DomainConnections.Remove(context);
            }
        }

        #region task service

        private void RegisterTaskQueueCacheEntry()
        {
            if (null != HttpContext.Current.Cache[TASK_QUEUE_CACHE_ITEM_KEY]) return;

            HttpContext.Current.Cache.Add(TASK_QUEUE_CACHE_ITEM_KEY, string.Empty, null,
                DateTime.MaxValue, TimeSpan.FromMinutes(1),
                CacheItemPriority.Normal,
                TaskQueueCacheItemRemovedCallback);

        }

        private void TaskQueueCacheItemRemovedCallback(string key,
            object value, CacheItemRemovedReason reason)
        {
            Log.Debug("Checking for queued tasks.");

            Refresh();

            ExecuteQueuedTasks();
        }

        /// <summary>
        /// Keeps the app from sleeping.
        /// </summary>
        private static void Refresh()
        {
            try
            {
                ServicePointManager.ServerCertificateValidationCallback += Certificate.AcceptAny;

                var client = new WebClient();
                client.DownloadData(TaskQueueCacheRefresherPageUrl);
            }
            catch (Exception ex)
            {
                Log.Fatal(string.Format("Unable to refresh cache using '{0}'.", TaskQueueCacheRefresherPageUrl), ex);
            }
        }

        /// <summary>
        /// TODO: Put tasks in the queue somewhere.
        /// </summary>
        protected virtual void ExecuteQueuedTasks()
        {
            var tasksPendingExecution = TaskQueue.Where(task => task.ExecutionTime <= DateTime.Now).ToList();

            if (tasksPendingExecution.Count == 0)
                Log.Debug("No tasks pending execution found.");

            foreach (var taskPendingExecution in tasksPendingExecution)
            {
                Log.Info(string.Format("Executing task '{0}', and removing from the queue.", taskPendingExecution.GetType()));
                lock (TaskQueue)
                {
                    TaskQueue.Remove(taskPendingExecution);
                }
                taskPendingExecution.Execute();
            }
        }

        #endregion
    }
}